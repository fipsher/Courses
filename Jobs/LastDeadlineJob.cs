using LNU.Courses.BLL.DeadlinesBLL;
using LNU.Courses.Repositories;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNU.Courses.Jobs
{
    public class LastDeadlineJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var EmailTemplate = @"Шановний студенте, вітаємо!
                                 Ви обрали дисципліни «{Disc_1}» для 3 семестру та
                                 «{Disc_2}» для 4 семестру {years} навчальний
                                 рік. Бажаємо успіхів у вивченні дисциплін!
                                 Зі списком груп за вибраними Вами дисциплінами можна ознайомитись 
                                <a href='http://194.44.198.79/Shared/DetailsOfDiscipline/{id_1}'>тут</a> та
                                <a href='http://194.44.198.79/Shared/DetailsOfDiscipline/{id_2}'>тут</a>";

            IRepository repository = new Repository();
            SecondDeadline sd = new SecondDeadline(repository);
            sd.FillGroupsWithRemainedStd(2);
            var students = repository.GetStudents(s => s.Deleted == false);

            foreach (var item in students)
            {
                try
                {
                    MailSender mailSender = new MailSender();
                    string subject = "ЛНУ Курси";
                    var em = new List<string>();
                    em.Add(item.eMail);
                    var discs = repository.GetStudentsInGroupsQuery().Where(s => s.studentID == item.id).Select(s => s.Group.Disciplines).ToList();

                    var body = EmailTemplate;
                    var i = 1;
                    foreach (var d in discs)
                    {
                        body = body.Replace("{Disc_" + i + "}", d.name).Replace("{id_" + i + "}", d.id.ToString());
                        i++;
                    }
                    var year = System.DateTime.Now.Year;
                    body = body.Replace("{years}", $"{year}/{year + 1}")
                               .Replace("{Disc_1}", "")
                               .Replace("{Disc_2}", "");
                    if (!string.IsNullOrEmpty(item.eMail))
                        mailSender.SendMail(subject, body, item.eMail);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public void ExecuteTemp()
        {
            var EmailTemplate = @"Шановний студенте, вітаємо!
                                 Ви обрали дисципліни «{Disc_1}» для 3 семестру та
                                 «{Disc_2}» для 4 семестру {years} навчальний
                                 рік. Бажаємо успіхів у вивченні дисциплін!
                                 Зі списком груп за вибраними Вами дисциплінами можна ознайомитись 
                                <a href='http://194.44.198.79/Shared/DetailsOfDiscipline/{id_1}'>тут</a> та
                                <a href='http://194.44.198.79/Shared/DetailsOfDiscipline/{id_2}'>тут</a>";

            IRepository repository = new Repository();
            SecondDeadline sd = new SecondDeadline(repository);
            sd.FillGroupsWithRemainedStd(2);
            var students = repository.GetStudents(s => s.Deleted == false);

            foreach (var item in students)
            {
                try
                {
                    MailSender mailSender = new MailSender();
                    string subject = "ЛНУ Курси";
                    var em = new List<string>();
                    em.Add(item.eMail);
                    var discs = repository.GetStudentsInGroupsQuery().Where(s => s.studentID == item.id).Select(s => s.Group.Disciplines).ToList();

                    var body = EmailTemplate;
                    var i = 1;
                    foreach (var d in discs)
                    {
                        body = body.Replace("{Disc_" + i + "}", d.name).Replace("{id_" + i + "}", d.id.ToString());
                        i++;
                    }
                    var year = System.DateTime.Now.Year;
                    body = body.Replace("{years}", $"{year}/{year + 1}")
                               .Replace("{Disc_1}", "")
                               .Replace("{Disc_2}", "");
                    if (!string.IsNullOrEmpty(item.eMail))
                        mailSender.SendMail(subject, body, item.eMail);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

        }
    }
}
