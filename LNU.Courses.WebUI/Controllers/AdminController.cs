using System;
using System.Linq;
using System.Web.Mvc;
using Entities;
using LNU.Courses;
using LNU.Courses.Jobs;
using LNU.Courses.PeriodicalJobMaker;
using LNU.Courses.Repositories;
using LNU.Courses.Security;
using LNU.Courses.WebUI.App_Start;
using LNU.Courses.WebUI.Models;
using LNU.Courses.BLL.RepoBLL;

namespace LNU.Courses.Controllers
{
    public class AdminController : Controller
    {
        private const int MaxStdOnPage = 100;
        private readonly IRepository _repository = new Repository();
        private readonly RepositoryBL _repoBl;

        public AdminController(IRepository repository)
        {
            _repository = repository;
            _repoBl = new RepositoryBL(_repository);
        }

        #region temp
        [AdminAuthorize(Roles = "SuperAdmin,Admin")]
        public ActionResult FirstDeadlineStart()
        {
            FirstDeadLineJob fdj = new FirstDeadLineJob();
            fdj.ExecuteTemp();
            return View("Index");
        }

        [AdminAuthorize(Roles = "SuperAdmin,Admin")]
        public ActionResult LastDeadlineStart()
        {
            LastDeadlineJob ldj = new LastDeadlineJob();
            ldj.ExecuteTemp();
            return View("Index");
        }
        #endregion


        public AdminController()
        {
            _repoBl = new RepositoryBL(_repository);
        }

        /// <summary>
        /// Main Admin page
        /// </summary>
        /// <returns></returns>
        [AdminAuthorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Index()
        {
            return View();
        }
        //[AdminAuthorize(Roles = "SuperAdmin,Admin")]
        //public ActionResult AboutDiscipline()
        //{
        //    return View();
        //}


        [AdminAuthorize(Roles = "SuperAdmin,Admin")]
        public ActionResult ChangePass()
        {
            ViewBag.ChangePassValidate = null;

            return View();
        }

        [HttpPost]
        [AdminAuthorize(Roles = "SuperAdmin,Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePass(string oldPass, string newPass, string checkNewPass)
        {
            var login = SessionPersister.Login;
            var adminAcc = _repoBl.GetAdmin(login);
            IHashProvider encoder = new HashProvider();

            if (adminAcc.password != encoder.Encrypt(oldPass))
            {
                ViewBag.ChangePassValidate = "Старий пароль вказано не вірно";
            }
            else
            {
                _repoBl.ChangeAdminPass(login, newPass);
            }
            return View();
        }
        [AdminAuthorize(Roles = "SuperAdmin")]
        public ActionResult ChangeCredentials()
        {
            ViewBag.Email = MailSender.EmailFrom;

            return View();
        }

        [HttpPost]
        [AdminAuthorize(Roles = "SuperAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeCredentials(string eMail, string password)
        {
            MailSender.EmailFrom = eMail;
            MailSender.Password = password;
            return View();
        }


        [AdminAuthorize(Roles = "SuperAdmin")]
        public ActionResult ChangeDeadLine()
        {
            ViewBag.Start = staticData.StartTime;
            ViewBag.FirstDeadline = staticData.firstDeadLineTime;
            ViewBag.LastDeadline = staticData.lastDeadLineTime;
            return View();
        }

        [HttpPost]
        [AdminAuthorize(Roles = "SuperAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeDeadLine(DateTime? start, DateTime? firstDeadline, DateTime? lastDeadline)
        {
            ViewBag.Result = false;
            if (start != null && firstDeadline != null && lastDeadline != null)
            {
                if (start < firstDeadline && firstDeadline < lastDeadline)
                {
                    ViewBag.Result = true;

                    DeadlineConfig.JobMaker.ClearSchedule();

                    DateParser dp = new DateParser();

                    string startPoint = dp.Parse(start);
                    DeadlineConfig.JobMaker.Start<StartJob>(startPoint);

                    string frstDeadline = dp.Parse(firstDeadline);
                    DeadlineConfig.JobMaker.Start<FirstDeadLineJob>(frstDeadline);

                    string lstDeadline = dp.Parse(lastDeadline);
                    DeadlineConfig.JobMaker.Start<LastDeadlineJob>(lstDeadline);
                    staticData.StartTime = new DateTime(start.Value.Year, start.Value.Month, start.Value.Day); //start;
                    staticData.firstDeadLineTime = new DateTime(firstDeadline.Value.Year, firstDeadline.Value.Month,
                        firstDeadline.Value.Day); // firstDeadline;
                    staticData.lastDeadLineTime = new DateTime(lastDeadline.Value.Year, lastDeadline.Value.Month,
                        lastDeadline.Value.Day); // lastDeadline;
                    staticData.disciplinesID = _repository.GetDisciplinesForSecondWave();

                }
            }

            ViewBag.Start = staticData.StartTime;
            ViewBag.FirstDeadline = staticData.firstDeadLineTime;
            ViewBag.LastDeadline = staticData.lastDeadLineTime;
            return View();
        }

        #region Admins managing

        [AdminAuthorize(Roles = "SuperAdmin")]
        public ActionResult GetAdmins()
        {
            return View();
        }

        [AdminAuthorize(Roles = "SuperAdmin")]
        public ActionResult EditAdmin(string login)
        {

            var admin = _repoBl.GetAdmin(login);

            return View(admin);
        }

        [HttpPost]
        [AdminAuthorize(Roles = "SuperAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult EditAdmin(Administrators adminAcc)
        {

            _repository.UpdateAdmin(adminAcc);

            return View(adminAcc);
        }

        [AdminAuthorize(Roles = "SuperAdmin")]
        public ActionResult CreateAdmin()
        {
            return View();
        }

        [HttpPost]
        [AdminAuthorize(Roles = "SuperAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAdmin(Administrators admin)
        {
            if (ModelState.IsValid)
            {
                ViewBag.AddAdminResult = _repository.AddAdmin(admin);
            }
            return View(admin);
        }

        [AdminAuthorize(Roles = "SuperAdmin")]
        public ActionResult DeleteAdmin(string login)
        {
            var admin = _repoBl.GetAdmin(login);
            return View(admin);
        }

        [HttpPost]
        [AdminAuthorize(Roles = "SuperAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAdmin(Administrators adminAcc)
        {
            _repository.DeleteAdmin(adminAcc.login);
            return View(adminAcc);
        }

        [ChildActionOnly]
        [AdminAuthorize(Roles = "SuperAdmin")]
        public PartialViewResult GetAdminsList()
        {
            var adminList = _repository.GetAdmins();
            return PartialView("_adminList", adminList);
        }
        #endregion

        #region Courses managing
        /// <summary>
        /// Get table of disciplines which contains param name in their names
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [AdminAuthorize(Roles = "SuperAdmin,Admin")]
        public ActionResult GetCourses(string name)
        {
            return View((object)name);
        }
        /// <summary>
        /// Edit discipline which discipline.id == id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AdminAuthorize(Roles = "SuperAdmin,Admin")]
        public ActionResult EditDiscipline(int id)
        {

            var disc = _repository.GetDiscipline(id);

            return View(disc);
        }

        /// <summary>
        /// Returns state of updating discipline 
        /// </summary>
        /// <param name="discipline"></param>
        /// <returns></returns>
        [HttpPost]
        [AdminAuthorize(Roles = "SuperAdmin,Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult EditDiscipline(Disciplines discipline)
        {
            if (ModelState.IsValid)
            {
                _repository.UpdateDiscipline(discipline);
            }
            return View(discipline);
        }

        /// <summary>
        /// Returns createDiscipline Viewe
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AdminAuthorize(Roles = "SuperAdmin,Admin")]
        public ActionResult CreateDiscipline()
        {
            return View();
        }

        /// <summary>
        /// Returns state of creating new discipline
        /// </summary>
        /// <param name="discipline"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorize(Roles = "SuperAdmin,Admin")]
        public ActionResult CreateDiscipline(Disciplines discipline)
        {
            if (ModelState.IsValid)
            {
                bool result = _repository.AddDiscipline(discipline);
                ViewBag.DisciplineAddResult = result;
            }
            return View();
        }


        [AdminAuthorize(Roles = "SuperAdmin, Admin")]
        public ActionResult DetailsOfDiscipline(int id)
        {
            var discipline = _repository.GetDiscipline(id);
            return View(discipline);
        }

        [HttpGet]
        [AdminAuthorize(Roles = "SuperAdmin, Admin")]
        public ActionResult DeleteDiscipline(int id)
        {
            var discipline = _repository.GetDiscipline(id);
            return View(discipline);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorize(Roles = "SuperAdmin, Admin")]
        public ActionResult DeleteDiscipline(Disciplines discipline)
        {
            ViewBag.DeleteDisciplineResult = _repository.DeleteDiscipline(discipline.id);

            return View("DeleteDiscipline", discipline);
        }

        /// <summary>
        /// Get students that registered on this discipline
        /// </summary>
        /// <param name="disciplineId"></param>
        /// <param name="wave"></param>
        /// <returns></returns>
        [ChildActionOnly]
        [AdminAuthorize(Roles = "SuperAdmin, Admin")]
        public PartialViewResult GetDisciplineStudents(int disciplineId, int wave)
        {
            var studentsList = _repoBl.GetStudents(disciplineId, wave);
            var admin = _repoBl.GetAdmin(SessionPersister.Login);
            if (admin.roles == "SuperAdmin")
                ViewBag.Admin = 1;

            return PartialView("_stList", studentsList);
        }

        /// <summary>
        /// partial viewe of disciplines
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [ChildActionOnly]
        [AdminAuthorize(Roles = "SuperAdmin,Admin")]
        public PartialViewResult GetDisciplinesList(string name)
        {
            var discList = _repoBl.GetDisciplines(name);
            var admin = _repoBl.GetAdmin(SessionPersister.Login);
            if (admin.roles == "SuperAdmin")
                ViewBag.Admin = 1;

            return PartialView("_discList", discList);
        }

        /// <summary>
        /// Get amount of students that assign to such discipline
        /// </summary>
        /// <param name="disciplineId"></param>
        /// <param name="wave"></param>
        /// <returns></returns>
        [ChildActionOnly]
        [AdminAuthorize(Roles = "SuperAdmin,Admin")]
        public PartialViewResult GetDisciplineStudentCount(int disciplineId, int wave)
        {
            var temp = _repository.GetGroupByDisciplinesId(disciplineId, wave);

            var studCount = temp?.AmountOfStudent ?? 0;
            return PartialView("_disciplineStudentAmount", studCount);
        }
        #endregion

        #region Students managing 

        /// <summary>
        /// Get list of students sorted by Students.fio
        /// </summary>
        /// <param name="fio"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        [HttpGet]
        [AdminAuthorize(Roles = "SuperAdmin")]
        public ActionResult GetStudents(string fio, string index = "1")
        {
            var ind = 1;
            int.TryParse(index, out ind);

            ViewBag.StudLength = (int)(_repoBl.GetStudents(fio).Count() / (double)MaxStdOnPage);
            ViewBag.index = ind + 1;
            return View((object)fio);
        }

        /// <summary>
        /// Edit Student who has student.id == id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AdminAuthorize(Roles = "SuperAdmin")]
        public ActionResult EditStudent(string id)
        {
            var student = _repoBl.GetStudentById(id);
            return View("EditStudent", student);
        }

        /// <summary>
        /// Take new Student's data to update
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorize(Roles = "SuperAdmin")]
        public ActionResult EditStudent(Students student)
        {
            if (ModelState.IsValid)
            {
                _repository.UpdateStudent(student);
            }
            return View("EditStudent", student);
        }

        /// <summary>
        /// Delete student who has Student.id = id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AdminAuthorize(Roles = "SuperAdmin")]
        public ActionResult DeleteStudent(string id)
        {
            var student = _repoBl.GetStudentById(id);
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorize(Roles = "SuperAdmin")]
        public ActionResult DeleteStudent(Students student)
        {
            ViewBag.DeleteStudentResult = _repository.DeleteStudent(student.id);

            return View("DeleteStudent", student);
        }

        /// <summary>
        /// Create new Student 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AdminAuthorize(Roles = "SuperAdmin")]
        public ActionResult CreateStudent()
        {
            return View();
        }

        /// <summary>
        /// Create new Student with such data
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorize(Roles = "SuperAdmin")]
        public ActionResult CreateStudent(Students student)
        {
            if (ModelState.IsValid)
            {
                bool result = _repository.AddStudent(student);
                ViewBag.StudentAddResult = result;
            }
            return View();
        }

        /// <summary>
        /// Partial view of students list who have student.fio == fio
        /// </summary>
        /// <param name="fio"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        [ChildActionOnly]
        [AdminAuthorize(Roles = "SuperAdmin")]
        public PartialViewResult GetStudentsList(string fio, string index = "1")
        {
            var studentsList = _repoBl.GetStudents(fio).ToList();
            var ind = 1;

            int.TryParse(index, out ind);
            if ((studentsList.Count / MaxStdOnPage) > ind)
            {
                int startP = ind * MaxStdOnPage - MaxStdOnPage;
                studentsList = studentsList.GetRange(startP, 100);

            }
            else
            {
                int amountToGet = studentsList.Count - (studentsList.Count / MaxStdOnPage) * MaxStdOnPage;
                int startP = (studentsList.Count / MaxStdOnPage) * MaxStdOnPage - MaxStdOnPage;
                if (startP > 0)
                {
                    studentsList = studentsList.GetRange(startP, amountToGet);
                }
                //ViewBag.EnabledNext = true;
            }
		if (_repoBl.GetAdmin(SessionPersister.Login).roles == "SuperAdmin")
                	ViewBag.Admin = 1;
            return PartialView("_stList", studentsList);
        }
        #endregion


    }
}