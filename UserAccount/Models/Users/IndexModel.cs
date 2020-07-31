using System;

namespace Zidium.UserAccount.Models.Users
{
    public class IndexModel
    {
        public UserInfo[] Users { get; set; }

        public class UserInfo
        {
            public Guid Id;

            public string Login;

            public string DisplayName;

            public string Post;

            public string Role;

        }
    }
}