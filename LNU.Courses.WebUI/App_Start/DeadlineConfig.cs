using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using LNU.Courses.Jobs;
using LNU.Courses.PeriodicalJobMaker;
using LNU.Courses.WebUI.Models;

namespace LNU.Courses.WebUI.App_Start
{
    public class DeadlineConfig
    {
        public static readonly IPeriodicalyJobMaker JobMaker = new JobMaker();

        public static void SetDeadlines()
        {
            JobMaker.ClearSchedule();

            string startPoint = WebConfigurationManager.AppSettings["Start"];
            string[] wordsStart = startPoint.Split(' ');
            JobMaker.Start<StartJob>(startPoint);
            staticData.StartTime = new DateTime(DateTime.Now.Year, Convert.ToInt32(wordsStart[4]), Convert.ToInt32(wordsStart[3]));


            string frstDeadline = WebConfigurationManager.AppSettings["FirstDeadLine"];
            string[] wordsFrst = frstDeadline.Split(' ');

            JobMaker.Start<FirstDeadLineJob>(frstDeadline);
            staticData.firstDeadLineTime = new DateTime(DateTime.Now.Year, Convert.ToInt32(wordsFrst[4]), Convert.ToInt32(wordsFrst[3]));

            string lastDeadline = WebConfigurationManager.AppSettings["LastDeadLine"];
            string[] wordsLast = lastDeadline.Split(' ');
            staticData.lastDeadLineTime = new DateTime(DateTime.Now.Year, Convert.ToInt32(wordsLast[4]), Convert.ToInt32(wordsLast[3]));
            JobMaker.Start<FirstDeadLineJob>(lastDeadline);
        }
    }
}