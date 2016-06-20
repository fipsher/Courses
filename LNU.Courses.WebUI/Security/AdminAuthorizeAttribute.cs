using System.Web.Mvc;
using System.Web.Routing;
using LNU.Courses.Repositories;
using LNU.Courses.WebUI.Models;

namespace LNU.Courses.Security
{
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        public IRepository _repository { get; set; }
        public IHashProvider _hashProvider { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (string.IsNullOrEmpty(SessionPersister.Login))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Account",
                    action = "AdminLogin"
                }));
            }
            else
            {
                AdminAccountModel am = new AdminAccountModel(_repository, _hashProvider);
                CustomPrincipal cp = new CustomPrincipal(am.Find(SessionPersister.Login));

                if (!cp.IsInRole(Roles))
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Home",
                        action = "Index"
                    }));
                }
            }
        }
    }

}