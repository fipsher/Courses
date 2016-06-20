using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;

namespace JobsMakers
{
    public class PeriodicallyJobMaker : IPeriodicallyJobMaker
    {
        public void Start<TJob>(string period) where TJob : class, IJob
        {
            try
            {
                Common.Logging.LogManager.Adapter = new Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter { Level = Common.Logging.LogLevel.Info };

                IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
                scheduler.Start();

                IJobDetail job = JobBuilder.Create<TJob>()
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                    .StartNow()
                    //.WithCronSchedule("0 0 12 1 7 ? *")//July 1 12:00
                    .WithCronSchedule(period)
                    //.WithCronSchedule("0/3 * * * * ?") //once per 3 seconds
                    .Build();
                scheduler.ScheduleJob(job, trigger);
            }
            catch (SchedulerException se)
            {
                Console.WriteLine(se);
            }
        }
    }
}