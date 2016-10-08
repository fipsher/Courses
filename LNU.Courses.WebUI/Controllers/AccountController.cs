using System.Web.Mvc;
using LNU.Courses.Models;
using LNU.Courses.Repositories;
using LNU.Courses.Security;
using LNU.Courses.WebUI.Models;

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