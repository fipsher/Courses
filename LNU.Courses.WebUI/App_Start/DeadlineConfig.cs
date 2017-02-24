using System;
using System.Web.Configuration;
using LNU.Courses.Jobs;
using LNU.Courses.PeriodicalJobMaker;
using LNU.Courses.WebUI.Models;
using Entities;
using System.Linq;

namespace LNU.Courses.WebUI.App_Start
{
    public class DeadlineConfig
    {
        public static readonly IPeriodicalyJobMaker JobMaker = new JobMaker();

        public static void SetDeadlines()
        {
            var context = new CoursesDataModel();
            JobMaker.ClearSchedule();
            DateParser dp = new DateParser();


            staticData.StartTime = context.Variables.Single(el => el.Key == "Start").Value; //new DateTime(DateTime.Now.Year, Convert.ToInt32(wordsStart[4]), Convert.ToInt32(wordsStart[3]));
            staticData.firstDeadLineTime = context.Variables.Single(el => el.Key == "FirstDeadline").Value; //new DateTime(DateTime.Now.Year, Convert.ToInt32(wordsFrst[4]), Convert.ToInt32(wordsFrst[3]));
            staticData.lastDeadLineTime = context.Variables.Single(el => el.Key == "LastDeadline").Value; //new DateTime(DateTime.Now.Year, Convert.ToInt32(wordsLast[4]), Convert.ToInt32(wordsLast[3]));

            string startPoint = dp.Parse(staticData.StartTime);
            DeadlineConfig.JobMaker.Start<StartJob>(startPoint);

            string frstDeadline = dp.Parse(staticData.firstDeadLineTime);
            DeadlineConfig.JobMaker.Start<FirstDeadLineJob>(frstDeadline);

            string lstDeadline = dp.Parse(staticData.lastDeadLineTime);
            DeadlineConfig.JobMaker.Start<LastDeadlineJob>(lstDeadline);
        }
    }
}