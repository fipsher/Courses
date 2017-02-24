using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LNU.Courses.WebUI.Models
{
    public class LecturerWithCredentials
    {
        [MinLength(3, ErrorMessage = "Мінімальна довжина повинна бути 3")]
        [Required(ErrorMessage = "Обов'язково до заповнення")]
        [Display(Name ="ПІБ")]
        public string fullName { get; set; }
        [Display(Name ="Номер телефону")]
        [Required(ErrorMessage = "Обов'язково до заповнення")]
        public string phone { get; set; }
        [Display(Name ="Лоігн")]
        [MinLength(3, ErrorMessage = "Мінімальна довжина повинна бути 3")]
        [Required(ErrorMessage = "Обов'язково до заповнення")]
        public string login { get; set; }
        [Display(Name ="Пароль")]
        [MinLength(3, ErrorMessage = "Мінімальна довжина повинна бути 3")]
        [Required(ErrorMessage = "Обов'язково до заповнення")]
        public string password { get; set; }

        public string roles { get; set; }
    }
}