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
                if (sessionVar != null)
                {
                    return sessionVar as string;
                }
                return null;
            }
            set
            {
                HttpContext.Current.Session[userSessionVar] = value;
            }
        }
    }
}