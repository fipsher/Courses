using LNU.Courses.BLL.DeadlinesBLL;
using LNU.Courses.BLL.RepoBLL;
using LNU.Courses.Jobs;
using LNU.Courses.Repositories;
using LNU.Courses.WebUI.App_Start;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace LNU.Courses
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DeadlineConfig.SetDeadlines();
            AutofacConfig.Initialize();
        }
    }
}
