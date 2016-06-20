using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(LNU.Courses.WebUI.App_Start.Startup))]
namespace LNU.Courses.WebUI.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
