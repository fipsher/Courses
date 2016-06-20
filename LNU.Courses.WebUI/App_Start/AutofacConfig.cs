using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using LNU.Courses.Repositories;

namespace LNU.Courses.WebUI.App_Start
{
    public static class AutofacConfig
    {
        public static void Initialize()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            builder.RegisterModule<LNU.Courses.Repositories.AutofacModule>();

            //builder.RegisterModule< LNU.Courses.PeriodicalJobMaker.AutofacModule>();
            builder.Register(c => new HashProvider()).As<IHashProvider>().InstancePerRequest();
            builder.RegisterFilterProvider();

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}