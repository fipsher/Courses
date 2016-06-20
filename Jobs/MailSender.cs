using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using LNU.Courses.Repositories;

namespace LNU.Courses.Jobs
{

    public class MailSender
    {
        private static string _emailFrom = "testsmtpagent@gmail.com";
        private static string _password = "10252525";

        public static string EmailFrom
        {
            get
            {
                return _emailFrom;
            }

            set
            {
                _emailFrom = value;
            }
        }

        public static string Password
        {
            get
            {
                return _password;
            }

            set
            {
                _password = value;
            }
        }

        public void SendMail(string subject, string body, List<string> eMails)
        {
            IRepository repository = new Repository();          

            SmtpClient smtp = new SmtpClient();
            MailMessage message = new MailMessage();
            //Message.From = new MailAddress(emailFrom);

            //List<string> eMails = repository.GetStudentEmails().ToList();
            
            for (int i =0; i < eMails.Count; i++)
            {
                if(eMails[i] !="")
                    try
                    {
                        message.To.Add(new MailAddress(eMails[i]));
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
            }
            message.From = new MailAddress(EmailFrom);
            smtp.Credentials = new NetworkCredential(EmailFrom, Password);
            message.IsBodyHtml = true;
            message.Subject = "subject";
            message.Body = body; //"Привіт!<br/> Прийшов Час зареєструватись на курси.";
            smtp.EnableSsl = true;
            smtp.Send(message);
        }
    }

}