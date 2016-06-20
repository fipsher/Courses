﻿using System;
using System.Linq;
using System.Security.Principal;
using LNU.Courses.Models;

namespace LNU.Courses.Security
{
    public class CustomPrincipal : IPrincipal
    {
        private readonly Account _account;

        public CustomPrincipal(Account account)
        {
            if (account != null)
            {
                _account = account;
                Identity = new GenericIdentity(_account.Login);
            }
        }

        public IIdentity Identity
        {
            get;
            set;
        }

        public bool IsInRole(string role)
        {
            var roles = role.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (_account != null)
            {
                return roles.Any(r => _account.Roles.Contains(r));
            }
            else
            {
                return false;
            }
        }
    }
}