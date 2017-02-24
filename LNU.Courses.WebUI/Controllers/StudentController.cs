﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Entities;
using LNU.Courses.BLL.RepoBLL;
using LNU.Courses.Repositories;
using LNU.Courses.Security;
using LNU.Courses.WebUI.Models;

namespace LNU.Courses.Controllers
{
    [StudentAuthorize(Roles = "User")]
    public class StudentController : Controller
    {
        private readonly IRepository repository;
        private readonly RepositoryBL _repoBl;

        public StudentController(IRepository repo)
        {
            repository = repo;
            _repoBl = new RepositoryBL(repository);
        }

        public StudentController()
        {
            _repoBl = new RepositoryBL(repository);
        }
        [StudentAuthorize(Roles = "User")]
        public ActionResult AboutDiscipline()
        {
            return View();
        }
        [StudentAuthorize(Roles = "User")]

        public ActionResult Welcome()
        {
            var studentAcc = _repoBl.GetStudentById(SessionPersister.Login);
            ViewBag.Fio = studentAcc.fio;
            return View();
        }
        public int GetDisciplineStudentCount(int disciplineId, int wave)
        {
            var studCount = _repoBl.GetStudents(disciplineId, wave).Count();
            return studCount;
        }
        // GET: Student
        [StudentAuthorize(Roles = "User")]
        public ActionResult Index()
        {
            if (repository.checkRegisteredPhoneNumber(SessionPersister.Login) == false)
                return View("EditPhone");
            if (repository.checkRegisteredEmail(SessionPersister.Login) == false)
                return View("EditEMail");

            return View();
        }
        public ActionResult EditEMail()
        {

            return View();
        }
        public ActionResult YouAreRegisted(int id)
        {
            if (repository.checkRegisteredPhoneNumber(SessionPersister.Login) == false)
                return View("EditPhone");
            if (repository.checkRegisteredEmail(SessionPersister.Login) == false)
                return View("EditEMail");

            return View(repository.GetDiscipline(id));
        }
        public ActionResult RegisteringInTheCourse()
        {
            if (repository.checkRegisteredPhoneNumber(SessionPersister.Login) == false)
                return View("EditPhone");
            if (repository.checkRegisteredEmail(SessionPersister.Login) == false)
                return View("EditEMail");


            return View();
        }
        [HttpGet]
        public ActionResult DeleteFromTheCourse(int id)
        {
            var context = new CoursesDataModel();

            staticData.StartTime = context.Variables.Single(el => el.Key == "Start").Value; //new DateTime(DateTime.Now.Year, Convert.ToInt32(wordsStart[4]), Convert.ToInt32(wordsStart[3]));
            staticData.firstDeadLineTime = context.Variables.Single(el => el.Key == "FirstDeadline").Value; //new DateTime(DateTime.Now.Year, Convert.ToInt32(wordsFrst[4]), Convert.ToInt32(wordsFrst[3]));
            staticData.lastDeadLineTime = context.Variables.Single(el => el.Key == "LastDeadline").Value; //new DateTime(DateTime.Now.Year, Convert.ToInt32(wordsLast[4]), Convert.ToInt32(wordsLast[3]));

            var wave = 0;
            int datetimeNowWithStart = DateTime.Compare(DateTime.Now, staticData.StartTime);
            int datetimeNowWithFirst = DateTime.Compare(DateTime.Now, staticData.firstDeadLineTime);
            int datetimeNowWithLast = DateTime.Compare(DateTime.Now, staticData.lastDeadLineTime);

            if (datetimeNowWithStart < 0 || datetimeNowWithLast > 0)
                wave = 0;
            else
                if (datetimeNowWithFirst < 0)
                wave = 1;
            else
                if (datetimeNowWithLast < 0)
                wave = 2;

            if (repository.checkRegisteredPhoneNumber(SessionPersister.Login) == false)
                return View("EditPhone");
            if (repository.checkRegisteredEmail(SessionPersister.Login) == false)
                return View("EditEMail");

            try
            {
                repository.DeleteMyDiscipline(id, SessionPersister.Login, wave);
                //repository.deleteAmountStudent(repository.GetGroupByDisciplinesId(id, wave).id);
            }
            catch
            {

            }
            return View("DeletedDicipline", repository.GetDiscipline(id));
        }
        [HttpGet]
        public ActionResult RegisteringInTheCourse(int id)
        {
            if (repository.checkRegisteredPhoneNumber(SessionPersister.Login) == false)
                return View("EditPhone");
            if (repository.checkRegisteredEmail(SessionPersister.Login) == false)
                return View("EditEMail");
            var context = new CoursesDataModel();

            staticData.StartTime = context.Variables.Single(el => el.Key == "Start").Value; //new DateTime(DateTime.Now.Year, Convert.ToInt32(wordsStart[4]), Convert.ToInt32(wordsStart[3]));
            staticData.firstDeadLineTime = context.Variables.Single(el => el.Key == "FirstDeadline").Value; //new DateTime(DateTime.Now.Year, Convert.ToInt32(wordsFrst[4]), Convert.ToInt32(wordsFrst[3]));
            staticData.lastDeadLineTime = context.Variables.Single(el => el.Key == "LastDeadline").Value; //new DateTime(DateTime.Now.Year, Convert.ToInt32(wordsLast[4]), Convert.ToInt32(wordsLast[3]));


            int datetimeNowWithStart = DateTime.Compare(DateTime.Now, staticData.StartTime);
            int datetimeNowWithFirst = DateTime.Compare(DateTime.Now, staticData.firstDeadLineTime);
            int datetimeNowWithLast = DateTime.Compare(DateTime.Now, staticData.lastDeadLineTime);
            var wave = 0;
            if (datetimeNowWithStart < 0 || datetimeNowWithLast > 0)
                wave = 0;
            else
                if (datetimeNowWithFirst < 0)
                wave = 1;
            else
                if (datetimeNowWithLast < 0)
                wave = 2;

            var disciplineAcc = repository.GetDiscipline(id);
            var studInGroup = new StudentsInGroups();
            var group = repository.GetGroupByDisciplinesId(disciplineAcc.id, wave);
            studInGroup.groupID = group.id;
            studInGroup.studentID = SessionPersister.Login;
            studInGroup.DateOfRegister = DateTime.Now;

            if (DateTime.Now.Year == group.year)
            {
                var student = context.Students.Single(st => st.id == SessionPersister.Login);
                var reg =  context.StudentsInGroups.Where(el => el.studentID == studInGroup.studentID && el.Group.Deleted == false)
                                .ToList();
                if (reg != null && reg.Count < 2 && reg.All(el => el.groupID != studInGroup.groupID))
                {
                    context.StudentsInGroups.Add(studInGroup);
                    context.SaveChanges();
                    repository.addAmountStudent(group.id);
                    return View("YouAreRegisted", disciplineAcc);
                }
            }
            return RedirectToAction("GetCourses", "Shared");
        }
        public ActionResult GetDisciplines(DisciplineSortingEnum? sortBy)
        {

            if (repository.checkRegisteredPhoneNumber(SessionPersister.Login) == false)
                return View("EditPhone");
            if (repository.checkRegisteredEmail(SessionPersister.Login) == false)
                return View("EditEMail");
            var context = new CoursesDataModel();

            staticData.StartTime = context.Variables.Single(el => el.Key == "Start").Value; //new DateTime(DateTime.Now.Year, Convert.ToInt32(wordsStart[4]), Convert.ToInt32(wordsStart[3]));
            staticData.firstDeadLineTime = context.Variables.Single(el => el.Key == "FirstDeadline").Value; //new DateTime(DateTime.Now.Year, Convert.ToInt32(wordsFrst[4]), Convert.ToInt32(wordsFrst[3]));
            staticData.lastDeadLineTime = context.Variables.Single(el => el.Key == "LastDeadline").Value; //new DateTime(DateTime.Now.Year, Convert.ToInt32(wordsLast[4]), Convert.ToInt32(wordsLast[3]));

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


            int checkReg = 0;
            var student = _repoBl.GetStudentById(SessionPersister.Login);

            IEnumerable<Disciplines> disc = repository.GetD(SessionPersister.Login);
            ViewBag.FirstSemestr = disc.Any(d => d.course == student.course * 2 + 1);
            ViewBag.SecondSemestr = disc.Any(d => d.course == student.course * 2 + 2);
            foreach (var item in disc)
            {
                if (item.course == _repoBl.GetStudentById(SessionPersister.Login).course)
                {
                    checkReg++;
                }
            }
            var studentAcc = _repoBl.GetStudentById(SessionPersister.Login);
            ViewBag.Course = studentAcc.course;
            ViewBag.CheckReg = checkReg;
            ViewBag.CheckLock = studentAcc.locked;
            ViewBag.Login = SessionPersister.Login;
            ViewBag.List = repository.GetD(SessionPersister.Login);
            ViewBag.NotDisciplines = repository.GetDisciplineWhereRegistered(SessionPersister.Login);

            List<Disciplines> disciplines = repository.GetDisciplinesSort(SessionPersister.Login, ViewBag.Wave);
            List<DisciplineViewModel> disciplinesSort = new List<DisciplineViewModel>();
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
            return View(disciplinesSort);
        }
        public ActionResult EditPhone()
        {

            return View();
        }

        public ActionResult ChangePassword()
        {
            if (repository.checkRegisteredPhoneNumber(SessionPersister.Login) == false)
                return View("EditPhone");
            if (repository.checkRegisteredEmail(SessionPersister.Login) == false)
                return View("EditEMail");


            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string oldPass, string newPass, string checkNewPass)
        {
            if (repository.checkRegisteredPhoneNumber(SessionPersister.Login) == false)
                return View("EditPhone");
            if (repository.checkRegisteredEmail(SessionPersister.Login) == false)
                return View("EditEMail");

            var login = SessionPersister.Login;
            var studentAcc = repository.GetUser(login);
            IHashProvider encoder = new HashProvider();

            if (studentAcc.password.ToUpper() != encoder.Encrypt(oldPass))
            {
                ViewBag.ChangePassValidate = "Старий пароль вказано не вірно";
            }
            else
            {
                repository.ChangeUserPass(login, newPass);
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEMail(string newEMail)
        {
            var login = SessionPersister.Login;
            var studentAcc = _repoBl.GetStudentById(login);

            if (studentAcc.eMail == newEMail)
            {
                ViewBag.ChangePassValidate = "OLD Email == NEW Email";
            }
            else
            {
                repository.ChangeStudentEMail(login, newEMail);
            }
            if (repository.checkRegisteredEmail(SessionPersister.Login) == false)
                return RedirectToAction("EditEMail", "Student", null);
            return RedirectToAction("GetCourses", "Shared");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPhone(string newPhone)
        {
            var login = SessionPersister.Login;
            var studentAcc = _repoBl.GetStudentById(login);

            if (studentAcc.phoneNumber == newPhone)
            {
                ViewBag.ChangePassValidate = "OLD Phone == NEW Phone";
            }
            else
            {
                repository.ChangeStudentPhone(login, newPhone);
            }
            if (repository.checkRegisteredPhoneNumber(SessionPersister.Login) == false)
                return RedirectToAction("EditPhone", "Student", null);
            if (repository.checkRegisteredEmail(SessionPersister.Login) == false)
                return RedirectToAction("EditEMail", "Student", null);

            return RedirectToAction("GetCourses", "Shared");
        }

        public ActionResult MyProfile()
        {
            if (repository.checkRegisteredPhoneNumber(SessionPersister.Login) == false)
                return View("EditPhone");
            if (repository.checkRegisteredEmail(SessionPersister.Login) == false)
                return View("EditEMail");

            var studentAcc = _repoBl.GetStudentById(SessionPersister.Login);
            ViewBag.Login = studentAcc.id;
            ViewBag.Fio = studentAcc.fio;
            ViewBag.Course = studentAcc.course;
            ViewBag.Group = studentAcc.group;
            ViewBag.AverageMark = studentAcc.AverageMark;
            ViewBag.EMail = studentAcc.eMail;
            ViewBag.PhoneNumber = studentAcc.phoneNumber;

            return View(repository.GetD(SessionPersister.Login));
        }

    }
}