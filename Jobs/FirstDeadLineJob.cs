using System.Collections.Generic;
using System.Linq;
using LNU.Courses.BLL.DeadlinesBLL;
using LNU.Courses.BLL.RepoBLL;
using LNU.Courses.Repositories;
using Quartz;

namespace LNU.Courses.Jobs
{
    public class FirstDeadLineJob : IJob
    {

        public void Execute(IJobExecutionContext context)
        {
            IRepository repository = new Repository();

            DeadlineManUp frst = new DeadlineManUp(repository);
            frst.ManUpGroups(1);
        }

        public void ExecuteTemp()
        {
            IRepository repository = new Repository();

            DeadlineManUp frst = new DeadlineManUp(repository);
            frst.ManUpGroups(1);
        }
    }
}
