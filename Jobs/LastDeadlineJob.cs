using LNU.Courses.BLL.DeadlinesBLL;
using LNU.Courses.Repositories;
using Quartz;

namespace LNU.Courses.Jobs
{
    public class LastDeadlineJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            IRepository repository = new Repository();
            SecondDeadline sd = new SecondDeadline(repository);
            sd.FillGroupsWithRemainedStd();

        }

        public void ExecuteTemp()
        {
            IRepository repository = new Repository();
            SecondDeadline sd = new SecondDeadline(repository);
            sd.FillGroupsWithRemainedStd();

        }
    }
}
