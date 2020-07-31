using System;

namespace Zidium.Core.ConfigDb
{
    public class LoginInfo
    {
        public Guid Id { get; set; }

        public string Login { get; set; }

        public Guid AccountId { get; set; }
    }
}
