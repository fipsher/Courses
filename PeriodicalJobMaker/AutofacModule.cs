using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace LNU.Courses.PeriodicalJobMaker
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<IPeriodicalyJobMaker>().As<JobMaker>();
        }
    }
}
