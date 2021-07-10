using System;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount
{
    public class UserInfo
    {
        public Guid Id { get; set; }

        public string Login { get; set; }

        public string Name { get; set; }

        public UserInfoRole[] Roles { get; set; }

        public bool IsSwitched { get; set; }
    }
}