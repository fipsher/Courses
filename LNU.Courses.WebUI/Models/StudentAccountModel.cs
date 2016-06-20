using LNU.Courses.Repositories;

namespace LNU.Courses.Models
{
    public class StudentAccountModel
    {
        private readonly IRepository repository = new Repository();
        readonly IHashProvider hashProvider = new HashProvider();

        public StudentAccountModel()
        {

        }

        public Account Find(string login)
        {
            var user = repository.GetUser(login);
            Account acc = new Account()
            {
                Login = user.login,
                Password = hashProvider.Encrypt(user.password),
                Roles = new string[] { "User" }
            };
            return acc;
        }

        public Account Login(string login, string password)
        {
            var user = repository.UserLogIn(login, password);
            if (user != null)
            {
                Account acc = new Account()
                {
                    Login = user.login,
                    Password = user.password, //hashProvider.Encrypt(user.password),
                    Roles = new string[] { "User" }
                };
                return acc;
            }

            return null;
        }
    }
}