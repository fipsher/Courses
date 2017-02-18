using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Entities;
using LNU.Courses.BLL.ExcelWriter;
using LNU.Courses.BLL.RepoBLL;
using LNU.Courses.Repositories;
using LNU.Courses.Security;
using LNU.Courses.WebUI.Models;

namespace LNU.Courses.WebUI.Controllers
{
    [AllowAnonymous]
    public class SharedController : Controller
    {
        private readonly IRepository _repository;
        private RepositoryBL _repoBl;

        public SharedController(IRepository repository)
        {
            _repository = repository;
            _repoBl = new RepositoryBL(_repository);
        }

        [ChildActionOnly]
        public PartialViewResult GetStudentStatistic()
        {
            var students = _repository.GetStudents().ToList();
            var notRegisteredStdCount = _repoBl.GetNotRegisteredStudents().Count;
            var onceRegisteredStd = _repoBl.GetOnceRegisteredStudents().Count;
            var registeredStudents = students.Count - notRegisteredStdCount - onceRegisteredStd;

            ViewBag.NotRegister = notRegisteredStdCount;
            ViewBag.RegisterOnce = onceRegisteredStd;
            ViewBag.FullRegistered = registeredStudents;

            return PartialView("_statistic", registeredStudents);
        }

        public ActionResult Info()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetCourses(string name, DisciplineSortingEnum? sortBy)
        {
            var temp = Session["Roles"];
            if (temp != null)
            {
                List<string> roles = ((string[])temp).ToList();
                if (roles.Contains("User"))
                {
                    //RedirectToAction("GetDisciplines", "Student");
                    if (_repository.checkRegisteredPhoneNumber(SessionPersister.Login) == false)
                        return RedirectToAction("EditPhone", "Student", null);
                    if (_repository.checkRegisteredEmail(SessionPersister.Login) == false)
                        return RedirectToAction("EditEMail", "Student", null);
                }                
            }
            ViewBag.SortBy = sortBy;
            return View((object)name);
        }

        [ChildActionOnly]
        public PartialViewResult GetDisciplinesList(string name, DisciplineSortingEnum? sortBy)
        {
            IEnumerable<Disciplines> discList;
            if (string.IsNullOrEmpty(name))
            {
                discList = _repository.GetDisciplines();
            }
            else
            {
                discList = _repoBl.GetDisciplines(d => d.name.ToLower().Contains(name.ToLower()));
            }
            var user = _repository.GetStudent(el => !string.IsNullOrEmpty(SessionPersister.Login) && el.id == SessionPersister.Login);
            if (user != null)
            {


                discList = discList.Where(d => d.course == user.course * 2 + 2 || d.course == user.course * 2 + 1);
                int datetimeNowWithStart = DateTime.Compare(DateTime.Now, staticData.StartTime);
                int datetimeNowWithFirst = DateTime.Compare(DateTime.Now, staticData.firstDeadLineTime);
                int datetimeNowWithLast = DateTime.Compare(DateTime.Now, staticData.lastDeadLineTime);

                if (datetimeNowWithStart < 0 || datetimeNowWithLast > 0)
                    ViewBag.Wave = 0;
                else
                    if (datetimeNowWithFirst < 0)
                    ViewBag.Wave = 1;
                else
                    if (datetimeNowWithLast < 0)
                    ViewBag.Wave = 2;
                ViewBag.CheckLock = user.locked;

                var stdRegisteredOn = _repository.GetUserRegisteredDisc(user.id);
                ViewBag.FirstSemestr = stdRegisteredOn.Any(d => d.course == user.course * 2 + 1);
                ViewBag.SecondSemestr = stdRegisteredOn.Any(d => d.course == user.course * 2 + 2);
            }
            List<Disciplines> disciplines = discList.ToList();
            List<DisciplineViewModel> disciplinesSort = new List<DisciplineViewModel>();
            var temp = Session["Roles"];
            if (temp != null)
            {
                List<string> roles = ((string[])temp).ToList();
                if (roles.Contains("Lecturer"))
                {
                    var lectId = _repoBl.GetAdmin(SessionPersister.Login).lecturerId;

                    List<Disciplines> tempDiscList =
                        _repository.GetDisciplines(el => el.lecturerId == lectId).ToList();
                    List<DisciplineViewModel> toViewBag = new List<DisciplineViewModel>();
                    tempDiscList.ForEach(el =>
                    {
                        toViewBag.Add(new DisciplineViewModel(el));
                        toViewBag.Last().firstWave = GetDisciplineStudentCount(el.id, 1);
                        toViewBag.Last().secondWave = GetDisciplineStudentCount(el.id, 2);
                    });
                    ViewBag.LecturerCourses = toViewBag;
                }
            }
            disciplines.ForEach(el =>
            {
                disciplinesSort.Add(new DisciplineViewModel(el));
                disciplinesSort.Last().firstWave = GetDisciplineStudentCount(el.id, 1);
                disciplinesSort.Last().secondWave = GetDisciplineStudentCount(el.id, 2);
            });
            if (sortBy.HasValue)
            {
                switch (sortBy)
                {
                    case DisciplineSortingEnum.Course:
                        {
                            disciplinesSort.Sort((el1, el2) => el1.course.CompareTo(el2.course));
                        }
                        break;
                    case DisciplineSortingEnum.Kafedra:
                        {
                            disciplinesSort.Sort((el1, el2) => String.Compare(el1.kafedra, el2.kafedra, StringComparison.Ordinal));
                        }                                           
                        break;
                    case DisciplineSortingEnum.Name:
                        {
                            disciplinesSort.Sort((el1, el2) => String.Compare(el1.name, el2.name, StringComparison.Ordinal));
                        }
                        break;
                    case DisciplineSortingEnum.FirstWave:
                        {
                            disciplinesSort.Sort((el1, el2) => el1.firstWave.CompareTo(el2.firstWave) * (-1));
                        }
                        break;
                    case DisciplineSortingEnum.SecondWave:
                        {
                            disciplinesSort.Sort((el1, el2) => el1.secondWave.CompareTo(el2.secondWave) * (-1));
                        }
                        break;
                }
            }

            ViewBag.Name = name;
            return PartialView("_discList", disciplinesSort);
        }

        public ActionResult DetailsOfDiscipline(int id, StudentSortingEnum? sortBy)
        {
            var discipline = _repository.GetDiscipline(id);
            ViewBag.Id = id;
            ViewBag.SortBy = sortBy;
            return View(discipline);
        }

        public int GetDisciplineStudentCount(int disciplineId, int wave)
        {
            var temp = _repository.GetGroupByDisciplinesId(disciplineId, wave);

            var studCount = temp?.AmountOfStudent ?? 0;
            return studCount;
        }

    }
}