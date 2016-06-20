using System;
using Quartz;
using Quartz.Impl;

namespace LNU.Courses.PeriodicalJobMaker
{
    public class JobMaker : IPeriodicalyJobMaker
    {
        readonly IScheduler _scheduler;

        public JobMaker()
        {
            _scheduler = StdSchedulerFactory.GetDefaultScheduler();
        }

        public void Start<TJob>(string dateString) where TJob : class, IJob
        {
            try
            {
                _scheduler.Start();

                IJobDetail job = JobBuilder.Create<TJob>()
                    //.WithIdentity("job", "group")
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    //.WithIdentity("trigger", "group")
                    .StartNow()
                    .WithCronSchedule(dateString)
                    .Build();

                _scheduler.ScheduleJob(job, trigger);

            }
            catch (SchedulerException se)
            {
                Console.WriteLine(se);
            }
        }

        public void ClearSchedule()
        {
            _scheduler.Clear();
        }
    }
}