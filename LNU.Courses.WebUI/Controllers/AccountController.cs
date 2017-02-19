using System.Web.Mvc;
using LNU.Courses.Models;
using LNU.Courses.Repositories;
using LNU.Courses.Security;
using LNU.Courses.WebUI.Models;
using LNU.Courses.BLL.RepoBLL;
using LNU.Courses.BLL.DeadlinesBLL;
using Entities;
using System;
using LNU.Courses.Jobs;
using System.Collections.Generic;
using System.Linq;

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
            //int wave = 1;
            //// removes old and adds new Groups 
            //repository.DeleteGroups();
            //repoBl.CreateNewGroups(wave);

            ////frst

            //var disciplineAcc = repository.GetDiscipline(91);
            //var studInGroup = new StudentsInGroups();
            //var group = repository.GetGroupByDisciplinesId(disciplineAcc.id);
            //studInGroup.groupID = group.id;
            //studInGroup.studentID = "1114010З ";
            //studInGroup.DateOfRegister = DateTime.Now;

            //if (DateTime.Now.Year == group.year)
            //    using (var context = new CoursesDataModel())
            //    {
            //        context.StudentsInGroups.Add(studInGroup);
            //        context.SaveChanges();
            //        repository.addAmountStudent(group.id);
            //    }
            //disciplineAcc = repository.GetDiscipline(120);
            //studInGroup = new StudentsInGroups();
            //group = repository.GetGroupByDisciplinesId(disciplineAcc.id);
            //studInGroup.groupID = group.id;
            //studInGroup.studentID = "1115024С ";
            //studInGroup.DateOfRegister = DateTime.Now;

            //if (DateTime.Now.Year == group.year)
            //    using (var context = new CoursesDataModel())
            //    {
            //        context.StudentsInGroups.Add(studInGroup);
            //        context.SaveChanges();
            //        repository.addAmountStudent(group.id);
            //    }

            //DeadlineManUp frst = new DeadlineManUp(repository);
            //frst.ManUpGroups(1);
            ////List<string> eMails = repoBl.GetStdEmailsForSecondWay().ToList();
            ////MailSender mailSender = new MailSender();
            ////string subject = "ЛНУ Курси";
            ////string body = "На жаль ти не зміг зареєструватись на вибраний курс.<br/> Але ти можеш зареєструватись на інший курс.";
            ////mailSender.SendMail(subject, body, eMails);

            //var EmailTemplate = @"Шановний студенте, вітаємо!
            //                     Ви обрали дисципліни «{Disc_1}» для 3 семестру та
            //                     «{Disc_2}» для 4 семестру {years} навчальний
            //                     рік. Бажаємо успіхів у вивченні дисциплін!
            //                     Зі списком груп за вибраними Вами дисциплінами можна ознайомитись 
            //                    <a href='http://194.44.198.79/Shared/DetailsOfDiscipline/{id_1}'>тут</a> та
            //                    <a href='http://194.44.198.79/Shared/DetailsOfDiscipline/{id_2}'>тут</a>";

            ////SecondDeadline sd = new SecondDeadline(repository);
            ////sd.FillGroupsWithRemainedStd(2);
            //var students = repository.GetStudents(s => s.Deleted == false).ToList();

            //foreach (var item in students)
            //{
            //    try
            //    {
            //        MailSender mailSender = new MailSender();
            //        string subject = "ЛНУ Курси";

            //        var context = new CoursesDataModel();
            //        var discs = context.StudentsInGroups.Where(s => s.studentID == item.id).Select(s => s.Group.Disciplines).ToList();

            //        var body = EmailTemplate;
            //        var i = 1;
            //        foreach (var d in discs)
            //        {
            //            body = body.Replace("{Disc_" + i + "}", d.name).Replace("{id_" + i + "}", d.id.ToString());
            //            i++;
            //        }
            //        var year = System.DateTime.Now.Year;
            //        body = body.Replace("{years}", $"{year}/{year + 1}")
            //                   .Replace("{Disc_1}", "")
            //                   .Replace("{Disc_2}", "");
            //        try
            //        {
            //            if (item.eMail != null)
            //                mailSender.SendMail(subject, body, item.eMail);
            //        }
            //        catch (Exception e)
            //        {
            //            Console.WriteLine(e);
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e);
            //    }
            //}
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