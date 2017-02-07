using System.Web.Mvc;
using LNU.Courses.Models;
using LNU.Courses.Repositories;
using LNU.Courses.Security;
using LNU.Courses.WebUI.Models;
using LNU.Courses.BLL.RepoBLL;
using LNU.Courses.BLL.DeadlinesBLL;
using Entities;
using System;

namespace LNU.Courses.Controllers
{
    public class AccountController : Controller
    {
        private readonly IRepository _repository;
        private readonly IHashProvider _hashProvider;
        public AccountController(IRepository repository, IHashProvider hashProvider)
        {
            _repository = repository;
            _hashProvider = hashProvider;
        }
        [HttpGet]
        public ActionResult Login()
        {

            IRepository repository = new Repository();
            RepositoryBL repoBl = new RepositoryBL(repository);
            int wave = 1;
            // removes old and adds new Groups 
            repository.DeleteGroups();
            repoBl.CreateNewGroups(wave);

            //frst

            var disciplineAcc = repository.GetDiscipline(8);
            var studInGroup = new StudentsInGroups();
            var group = repository.GetGroupByDisciplinesId(disciplineAcc.id);
            studInGroup.groupID = group.id;
            studInGroup.studentID = "1114010З ";
            studInGroup.DateOfRegister = DateTime.Now;

            if (DateTime.Now.Year == group.year)
                using (var context = new CoursesDataModel())
                {
                    context.StudentsInGroups.Add(studInGroup);
                    context.SaveChanges();
                    repository.addAmountStudent(group.id);
                }

            DeadlineManUp frst = new DeadlineManUp(repository);
            frst.ManUpGroups(1);
            //List<string> eMails = repoBl.GetStdEmailsForSecondWay().ToList();
            //MailSender mailSender = new MailSender();
            //string subject = "ЛНУ Курси";
            //string body = "На жаль ти не зміг зареєструватись на вибраний курс.<br/> Але ти можеш зареєструватись на інший курс.";
            //mailSender.SendMail(subject, body, eMails);

            SecondDeadline sd = new SecondDeadline(repository);
            sd.FillGroupsWithRemainedStd(2);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string login, string password)
        {

            StudentAccountModel am = new StudentAccountModel();

            if (ModelState.IsValid)
            {
                Account acc = am.Login(login, password);
                if (acc != null)
                {
                    SessionPersister.Login = acc.Login;
                    Session.Add("Roles", acc.Roles);
                    return RedirectToAction("Welcome", "Student");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
 
        }

        [HttpGet]
        public ActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdminLogin(string login, string password)
        {
            AdminAccountModel am = new AdminAccountModel();          

            if (ModelState.IsValid)
            {
                Account acc = am.Login(login, password);
                if (acc != null)
                {
                    SessionPersister.Login = acc.Login;                  

                    Session.Add("Roles", acc.Roles);
                    return RedirectToAction("GetCourses", "Shared");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }


        public ActionResult Logout()
        {
            SessionPersister.Login = string.Empty;
            Session.RemoveAll();
            return new RedirectResult("Login");
        }

        public ActionResult AccessDenied()
        {
            return View();
        }
    }
}