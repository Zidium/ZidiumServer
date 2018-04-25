using System;
using Zidium.Core.Api;

namespace Zidium.Core.ConfigDb
{
    public class LoginInfo
    {
        public Guid Id { get; set; }

        public string Login { get; set; }

        public AccountInfo Account { get; set; }
    }
}
