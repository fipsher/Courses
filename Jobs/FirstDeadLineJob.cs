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
            RepositoryBL repoBl = new RepositoryBL(repository);

            DeadlineManUp frst = new DeadlineManUp(repository);
            frst.ManUpGroups();
            //repository.ManUpGroups();

            //repository.DetruncateStudentsFirstWave();

            List<string> eMails = repoBl.GetStdEmailsForSecondWay().ToList();

            MailSender mailSender = new MailSender();
            string subject = "ЛНУ Курси";
            string body = "На жаль ти не зміг зареєструватись на вибраний курс.<br/> Але ти можеш зареєструватись на інший курс.";

            mailSender.SendMail(subject, body, eMails);
        }

        public void ExecuteTemp()
        {
            IRepository repository = new Repository();
            RepositoryBL repoBl = new RepositoryBL(repository);

            DeadlineManUp frst = new DeadlineManUp(repository);
            frst.ManUpGroups();
            //repository.ManUpGroups();

            //repository.DetruncateStudentsFirstWave();

            List<string> eMails = repoBl.GetStdEmailsForSecondWay().ToList();

            MailSender mailSender = new MailSender();
            string subject = "ЛНУ Курси";
            string body = "На жаль ти не зміг зареєструватись на вибраний курс.<br/> Але ти можеш зареєструватись на інший курс.";

            mailSender.SendMail(subject, body, eMails);
        }
    }
}
