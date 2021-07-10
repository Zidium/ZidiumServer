using System;

namespace Zidium.UserAccount.Models.Users
{
    public class ChangePasswordEndModel
    {
        public Guid UserId { get; set; }

        public string UserName { get; set; }
    }
}