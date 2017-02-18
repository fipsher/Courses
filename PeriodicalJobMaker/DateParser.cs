using System;

namespace LNU.Courses.PeriodicalJobMaker
{
    public class DateParser
    {
        public string Parse(DateTime? date)
        {
            string template = "0 {0} {1} {2} {3} ? *";

            if (date != null)
            {

                return string.Format(template, date.Value.Minute, date.Value.Hour, date.Value.Day, date.Value.Month);
            }
            else
            {
                return "";
            }
        }
    }
}
