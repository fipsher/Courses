using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LNU.Courses.WebUI.Models
{
    public class LecturerWithCredentials
    {
        public string fullName { get; set; }
        public string phone { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public string roles { get; set; }
    }
}