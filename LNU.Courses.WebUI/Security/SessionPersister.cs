using System.Web;

namespace LNU.Courses.Security
{
    public static class SessionPersister
    {
        static readonly string userSessionVar = "username";

        public static string Login
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return string.Empty;
                }

                var sessionVar = HttpContext.Current.Session[userSessionVar];
                return sessionVar as string;
            }
            set
            {
                HttpContext.Current.Session[userSessionVar] = value;
            }
        }
    }
}