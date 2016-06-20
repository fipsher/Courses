using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Integration.Mvc;

namespace LNU.Courses.Repositories
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Repository>().As<IRepository>();
           // builder.RegisterControllers(System.Reflection.Assembly.GetExecutingAssembly());

        }
    }
}
