using System.ComponentModel.DataAnnotations;

namespace LNU.Courses.Models
{
    public class Account
    {
        [Required(ErrorMessage = "Введіть логін")]
        [Display(Name = "Логін")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Введіть пароль")]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        public string[] Roles { get; set; }
    }
}