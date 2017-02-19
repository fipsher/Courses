using System;
using System.Net;
using System.Net.Mail;

namespace LNU.Courses.Jobs
{

    public class MailSender
    {
        private static string _emailFrom = "testsmtpagent@gmail.com";
        private static string _password = "10252525";
        private SmtpClient client;
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

        public MailSender()
        {
            client = new SmtpClient();
            client.UseDefaultCredentials = true;
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential(_emailFrom, _password);
            client.Timeout = 20000;
           
        }

        public void SendMail(string subject, string body, string eMail)
        {
            MailMessage msg = new MailMessage();
            msg.IsBodyHtml = true;
            msg.To.Add(eMail);
            msg.Subject = subject;
            msg.Body = body;
            msg.From = new MailAddress(_emailFrom);
            try
            {
                client.Send(msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                msg.Dispose();
            }
        }

    }

}