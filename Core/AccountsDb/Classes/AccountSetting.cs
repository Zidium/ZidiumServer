using System;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Настройка аккаунта
    /// </summary>
    public class AccountSetting
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}
