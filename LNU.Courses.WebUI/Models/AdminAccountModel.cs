using System;
using System.Collections.Generic;
using LNU.Courses.BLL.RepoBLL;
using LNU.Courses.Models;
using LNU.Courses.Repositories;

namespace LNU.Courses.WebUI.Models
{
    public class AdminAccountModel
    {
        private readonly IRepository _repository = new Repository();
        private List<Account> _listAccount = new List<Account>();
        private readonly IHashProvider _hashProvider = new HashProvider();
        private readonly RepositoryBL _repoBl;

        public AdminAccountModel()
        {
            _repoBl = new RepositoryBL(_repository);
        }

        public Account Find(string login)
        {
            var admin = _repoBl.GetAdmin(login);
            Account acc = new Account()
            {
                Login = admin.login,
                Password = _hashProvider.Encrypt(admin.password),
                Roles = admin.roles.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
            };
            return acc;
        }

        public Account Login(string login, string password)
        {
            var admin = _repoBl.AdminLogIn(login, password);
            if (admin != null)
            {
                Account acc = new Account()
                {
                    Login = admin.login,
                    Password = _hashProvider.Encrypt(admin.password),
                    Roles = admin.roles.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                };
                return acc;
            }

            return null;
        }
    }
}