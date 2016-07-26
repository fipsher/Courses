using System.Web.Mvc;
using System.Web.Routing;
using LNU.Courses.Models;

namespace LNU.Courses.Security
{
    public class StudentAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (string.IsNullOrEmpty(SessionPersister.Login))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Account",
                    action = "Login"
                }));
            }
            else
            {
                StudentAccountModel am = new StudentAccountModel();
                CustomPrincipal cp = new CustomPrincipal(am.Find(SessionPersister.Login));

                if (!cp.IsInRole(Roles))
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Account",
                        action = "AccessDenied"
                    }));
                }
            }
        }
    }
}