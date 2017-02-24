using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Entities;
using LNU.Courses;
using LNU.Courses.BLL.ExcelWriter;
using LNU.Courses.Jobs;
using LNU.Courses.PeriodicalJobMaker;
using LNU.Courses.Repositories;
using LNU.Courses.Security;
using LNU.Courses.WebUI.App_Start;
using LNU.Courses.WebUI.Models;
using LNU.Courses.BLL.RepoBLL;
using System.Configuration;
using System.Web.Configuration;

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

        [HttpGet]
        public ActionResult ChangePhoneNumber()
        {
            return View("ChangeLectPhoneNumb");
        }

        [HttpPost]
        [AdminAuthorize(Roles = "Lecturer")]
        public ActionResult ChangePhoneNumber(string phoneNumb)
        {
            try
            {
                var login = SessionPersister.Login;
                var singleOrDefault = _repository.GetAdmins().SingleOrDefault(el => el.login == login);
                if (singleOrDefault != null)
                {
                    var lectID = singleOrDefault.lecturerId;
                    var lecturer = _repository.GetLecturers().SingleOrDefault(el => el.Id == lectID);
                    if (lecturer != null)
                    {
                        lecturer.phone = phoneNumb;
                        _repository.UpdateLecturer(lecturer);

                    }
                }

            }
            catch
            {
                // ignored
            }
            return View("GetCourses");
        }

        #region temp
        [AdminAuthorize(Roles = "SuperAdmin,Admin")]
        public ActionResult Start()
        {
            _repository.DeleteGroups();
            _repoBl.CreateNewGroups(1);

            return View("Index");
        }

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
        [ChildActionOnly]
        [AdminAuthorize(Roles = "SuperAdmin,Admin")]
        public string GetLecturerList(int? lecturerId)
        {
            var lectList = _repository.GetLecturers().ToList();
            StringBuilder result = new StringBuilder();
            result.Append("<select name = 'lecturerId' class='btn btn-default dropdown-toggle'> ");
            result.Append($"<option value='{null}'>{null}</option>");
            lectList.ForEach(el =>
            {
                string selected = el.Id == lecturerId ? "selected" : "";
                result.Append($"<option {selected} value='{el.Id}'>{el.fullName}</option>");
            });
            result.Append("</select>");

            return result.ToString();
        }

        [AdminAuthorize(Roles = "SuperAdmin,Admin")]
        public ActionResult DeleteLecturer(int id)
        {
            var lecturer = _repository.GetLecturers().SingleOrDefault(el => el.Id == id);
            return View(lecturer);
        }

        [HttpPost]
        [AdminAuthorize(Roles = "SuperAdmin,Admin")]
        public ActionResult DeleteLecturer(Lecturer lecturer)
        {
            ViewBag.Status = _repository.DeleteLecturer(lecturer);

            return View(lecturer);
        }


        [AdminAuthorize(Roles = "SuperAdmin,Admin")]
        public ActionResult GetLecturers()
        {
            var lecturers = _repository.GetLecturers();
            return View(lecturers);
        }

        [AdminAuthorize(Roles = "SuperAdmin,Admin")]
        public ActionResult CreateLecturer()
        {
            return View();
        }

        [HttpPost]
        [AdminAuthorize(Roles = "SuperAdmin,Admin")]
        public ActionResult CreateLecturer(LecturerWithCredentials lecturer)
        {
            if (ModelState.IsValid)
            {
                Lecturer lect = new Lecturer()
                {
                    fullName = lecturer.fullName,
                    phone = lecturer.phone
                };
                Administrators admin = new Administrators()
                {
                    login = lecturer.login,
                    password = lecturer.password,
                    Lecturers = lect,
                    roles = "Lecturer"
                };

                _repository.AddAdmin(admin);
                return RedirectToAction("GetLecturers", "Admin");
            }
            return View();
        }

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


        [AdminAuthorize(Roles = "SuperAdmin,Admin,Lecturer")]
        public ActionResult ChangePass()
        {
            ViewBag.ChangePassValidate = null;

            return View();
        }

        [HttpPost]
        [AdminAuthorize(Roles = "SuperAdmin,Admin,Lecturer")]
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
            var context = new CoursesDataModel();

            staticData.StartTime = context.Variables.Single(el => el.Key == "Start").Value; //new DateTime(DateTime.Now.Year, Convert.ToInt32(wordsStart[4]), Convert.ToInt32(wordsStart[3]));
            staticData.firstDeadLineTime = context.Variables.Single(el => el.Key == "FirstDeadline").Value; //new DateTime(DateTime.Now.Year, Convert.ToInt32(wordsFrst[4]), Convert.ToInt32(wordsFrst[3]));
            staticData.lastDeadLineTime = context.Variables.Single(el => el.Key == "LastDeadline").Value; //new DateTime(DateTime.Now.Year, Convert.ToInt32(wordsLast[4]), Convert.ToInt32(wordsLast[3]));

            ViewBag.Start = staticData.StartTime;
            ViewBag.FirstDeadline = staticData.firstDeadLineTime;
            ViewBag.LastDeadline = staticData.lastDeadLineTime;
            return View();
        }

        [HttpPost]
        [AdminAuthorize(Roles = "SuperAdmin")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeDeadLine(DateTime? start, DateTime? firstDeadline, DateTime? lastDeadline, DateTime startTime, DateTime firstDeadlineTime, DateTime lastDeadlineTime)
        {
            ViewBag.Result = false;
            if (start != null && firstDeadline != null && lastDeadline != null)
            {
                if (start < firstDeadline && firstDeadline < lastDeadline)
                {
                    ViewBag.Result = true;
                    staticData.StartTime = new DateTime(start.Value.Year, start.Value.Month,
                        start.Value.Day, startTime.Hour, startTime.Minute, 0); //start;
                    staticData.firstDeadLineTime = new DateTime(firstDeadline.Value.Year, firstDeadline.Value.Month,
                        firstDeadline.Value.Day, firstDeadlineTime.Hour, firstDeadlineTime.Minute, 0); // firstDeadline;
                    staticData.lastDeadLineTime = new DateTime(lastDeadline.Value.Year, lastDeadline.Value.Month,
                        lastDeadline.Value.Day, lastDeadlineTime.Hour, lastDeadlineTime.Minute, 0); // lastDeadline;
                    staticData.disciplinesID = _repository.GetDisciplinesForSecondWave();

                    var context = new CoursesDataModel();
                    var StartTime = context.Variables.Single(el => el.Key == "Start");
                    var firstDeadLineTime = context.Variables.Single(el => el.Key == "FirstDeadline");
                    var lastDeadLineTime = context.Variables.Single(el => el.Key == "LastDeadline");


                    StartTime.Value = staticData.StartTime;
                    firstDeadLineTime.Value = staticData.firstDeadLineTime;
                    lastDeadLineTime.Value = staticData.lastDeadLineTime;

                    context.SaveChanges();

                    DeadlineConfig.JobMaker.ClearSchedule();
                    DateParser dp = new DateParser();

                    string startPoint = dp.Parse(staticData.StartTime);
                    DeadlineConfig.JobMaker.Start<StartJob>(startPoint);

                    string frstDeadline = dp.Parse(staticData.firstDeadLineTime);
                    DeadlineConfig.JobMaker.Start<FirstDeadLineJob>(frstDeadline);

                    string lstDeadline = dp.Parse(staticData.lastDeadLineTime);
                    DeadlineConfig.JobMaker.Start<LastDeadlineJob>(lstDeadline);
                }
            }

            ViewBag.Start = staticData.StartTime;
            ViewBag.FirstDeadline = staticData.firstDeadLineTime;
            ViewBag.LastDeadline = staticData.lastDeadLineTime;
            return View();
        }

        private string ParseToQuartzFormat(string pattern, DateTime? time)
        {
            var result = string.Format(pattern, time.Value.Minute, time.Value.Hour, time.Value.Day, time.Value.Month);
            return result;
        }

        #region Admins managing

        [AdminAuthorize(Roles = "SuperAdmin,Admin")]
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
            ViewBag.Status = false;

            if (ModelState.IsValid)
            {
                ViewBag.Status = true;
                _repository.UpdateAdmin(adminAcc);
            }
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
        [AdminAuthorize(Roles = "SuperAdmin,Admin")]
        public PartialViewResult GetAdminsList()
        {
            var adminList = _repository.GetAdmins().Where(el => !el.roles.Contains("Lecturer"));
            return PartialView("_adminList", adminList);
        }
        #endregion

        #region Courses managing

        /// <summary>
        /// Edit discipline which discipline.id == id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AdminAuthorize(Roles = "SuperAdmin,Admin, Lecturer")]
        public ActionResult EditDiscipline(int id)
        {
            var login = SessionPersister.Login;
            var roles = (string[])Session["Roles"];
            bool flag = true;
            var discipline = _repository.GetDiscipline(id);

            if (roles.Contains("Lecturer"))
            {
                var lectorId = _repoBl.GetAdmin(login).lecturerId;
                if (discipline.lecturerId != lectorId)
                {
                    flag = false;
                }
            }

            if (flag)
            {
                ViewBag.SuperAdmin = _repoBl.GetAdmin(SessionPersister.Login).roles.ToLower().Contains("superadmin");
                var disc = _repository.GetDiscipline(id);
                var lectId = _repoBl.GetAdmin(login).lecturerId;
                disc.Lecturers = _repository.GetLecturers().SingleOrDefault(el => el.Id == lectId);
                return View(disc);
            }

            return RedirectToAction("AccessDenied", "Account");
        }

        /// <summary>
        /// Returns state of updating discipline 
        /// </summary>
        /// <param name="discipline"></param>
        /// <returns></returns>
        [HttpPost]
        [AdminAuthorize(Roles = "SuperAdmin,Admin, Lecturer")]
        [ValidateAntiForgeryToken]
        public ActionResult EditDiscipline(Disciplines discipline)
        {
            if (ModelState.IsValid)
            {
                if (discipline.Lecturers == null)
                {
                    //discipline.lecturerId = null;
                }
                _repository.UpdateDiscipline(discipline);
            }
            return View("GetCourses");
        }

        /// <summary>
        /// Returns createDiscipline Viewe
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AdminAuthorize(Roles = "SuperAdmin")]
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
        [AdminAuthorize(Roles = "SuperAdmin")]
        public ActionResult CreateDiscipline(Disciplines discipline)
        {
            if (ModelState.IsValid)
            {
                bool result = _repository.AddDiscipline(discipline);
                ViewBag.DisciplineAddResult = result;
            }
            return View();
        }


        [AdminAuthorize(Roles = "SuperAdmin, Admin, Lecturer")]
        public ActionResult DetailsOfDiscipline(int id, StudentSortingEnum? sortBy)
        {
            var discipline = _repository.GetDiscipline(id);
            ViewBag.Id = id;
            ViewBag.SortBy = sortBy;
            return View(discipline);
        }

        [HttpGet]
        [AdminAuthorize(Roles = "SuperAdmin")]
        public ActionResult DeleteDiscipline(int id)
        {
            var discipline = _repository.GetDiscipline(id);
            return View(discipline);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthorize(Roles = "SuperAdmin")]
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
        public PartialViewResult GetDisciplineStudents(int disciplineId, int wave, StudentSortingEnum? sortBy)
        {
            var studentsList = _repoBl.GetStudents(disciplineId, wave).ToList();
            ViewBag.Id = disciplineId;
            if (sortBy.HasValue)
            {
                switch (sortBy)
                {
                    case StudentSortingEnum.AverageMark:
                        {
                            studentsList.Sort((el1, el2) => el1.AverageMark.CompareTo(el2.AverageMark));
                        }
                        break;
                    case StudentSortingEnum.Course:
                        {
                            studentsList.Sort((el1, el2) => el1.course.CompareTo(el2.course));
                        }
                        break;
                    case StudentSortingEnum.Fio:
                        {
                            studentsList.Sort((el1, el2) => String.CompareOrdinal(el1.fio, el2.fio));
                        }
                        break;
                    case StudentSortingEnum.Group:
                        {
                            studentsList.Sort((el1, el2) => String.CompareOrdinal(el1.group, el2.group));
                        }
                        break;
                }
            }
            var admin = _repoBl.GetAdmin(SessionPersister.Login);
            if (admin.roles == "SuperAdmin")
                ViewBag.Admin = 1;
            return PartialView("_DisciplineStudents", studentsList);
        }


        /// <summary>
        /// Get amount of students that assign to such discipline
        /// </summary>
        /// <param name="disciplineId"></param>
        /// <param name="wave"></param>
        /// <returns></returns>
        [AdminAuthorize(Roles = "SuperAdmin,Admin")]
        public int GetDisciplineStudentCount(int disciplineId, int wave)
        {
            var temp = _repository.GetGroupByDisciplinesId(disciplineId, wave);

            var studCount = temp?.AmountOfStudent ?? 0;
            return studCount;
        }
        #endregion

        #region Students managing 

        /// <summary>
        /// Get list of students sorted by Students.fio
        /// </summary>
        /// <param name="fio"></param>
        /// <param name="index"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        [HttpGet]
        [AdminAuthorize(Roles = "SuperAdmin")]
        public ActionResult GetStudents(string fio, StudentSortingEnum? sortBy, string index = "1")
        {
            var ind = 1;
            int.TryParse(index, out ind);
            ViewBag.SortBy = sortBy;
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
            ViewBag.Status = false;

            if (ModelState.IsValid)
            {
                _repository.UpdateStudent(student);
                ViewBag.Status = true;
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
            var context = new CoursesDataModel();
            var student = context.Students.SingleOrDefault(el => el.id == id);
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
        /// <param name="sortBy"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        [ChildActionOnly]
        [AdminAuthorize(Roles = "SuperAdmin")]
        public PartialViewResult GetStudentsList(string fio, StudentSortingEnum? sortBy, string index = "1")
        {
            var studentsList = _repoBl.GetStudents(fio).ToList();
            var ind = 1;

            if (sortBy.HasValue)
            {
                switch (sortBy)
                {
                    case StudentSortingEnum.AverageMark:
                        {
                            studentsList.Sort((el1, el2) => el1.AverageMark.CompareTo(el2.AverageMark));
                        }
                        break;
                    case StudentSortingEnum.Course:
                        {
                            studentsList.Sort((el1, el2) => el1.course.CompareTo(el2.course));
                        }
                        break;
                    case StudentSortingEnum.Fio:
                        {
                            studentsList.Sort((el1, el2) => String.CompareOrdinal(el1.fio, el2.fio));
                        }
                        break;
                    case StudentSortingEnum.Group:
                        {
                            studentsList.Sort((el1, el2) => String.CompareOrdinal(el1.group, el2.group));
                        }
                        break;
                }
            }
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
            ViewBag.Fio = fio;
            return PartialView("_stList", studentsList);
        }
        #endregion


        [HttpGet]
        [AdminAuthorize(Roles = "SuperAdmin,Admin")]
        public async Task<ActionResult> Download(int disciplineId)
        {
            var discipline = _repository.GetDiscipline(disciplineId);
            var studentsList = _repoBl.GetStudents(disciplineId, 1).ToList();
            studentsList.ForEach(st =>
            {
                st.Users = null;
                st.StudentsInGroups = null;
            });
            var temp = _repoBl.GetStudents(disciplineId, 2).ToList();
            studentsList.AddRange(temp);

            var writer = new ExcelWriter<Students>(discipline.name);
            var fileBytes = writer.WriteToExcel(studentsList);

            var fileContent = await DownloadData(fileBytes, discipline.name);

            return fileContent;
        }

        private Task<FileContentResult> DownloadData(byte[] fileBytes, string fileName)
        {
            return Task.FromResult(File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, $"{fileName}.xlsx"));
        }
    }
}