using System;

namespace Zidium.UserAccount.Models.Users
{
    public class ChangePasswordModel
    {
        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string PasswordConfirm { get; set; }

        public string ErrorMessage { get; set; }
    }
}