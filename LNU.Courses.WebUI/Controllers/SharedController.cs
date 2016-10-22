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
                    RedirectToAction("GetDisciplines", "Student");
                }                
            }
            ViewBag.SortBy = sortBy;
            return View((object)name);
        }

        [ChildActionOnly]
        public PartialViewResult GetDisciplinesList(string name, DisciplineSortingEnum? sortBy)
        {
            var discList = _repoBl.GetDisciplines(name);
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
                        _repository.GetDisciplines().Where(el => el.lecturerId == lectId).ToList();
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