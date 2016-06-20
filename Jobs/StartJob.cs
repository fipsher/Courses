using System.Collections.Generic;
using System.Linq;
using LNU.Courses.BLL.RepoBLL;
using LNU.Courses.Repositories;
using Quartz;

namespace LNU.Courses.Jobs
{
    public class StartJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            IRepository repository = new Repository();
            RepositoryBL repoBl = new RepositoryBL(repository);
            int wave = 1;
            // removes old and adds new Groups 
            repository.DeleteGroups();
            repoBl.CreateNewGroups(wave);

            MailSender mailSender = new MailSender();
            List<string> eMails = repoBl.GetStudentEmails().ToList();
            mailSender.SendMail("ЛНУ Курси", "Привіт!<br/> Прийшов Час зареєструватись на курси.", eMails);

        }
    }
}
