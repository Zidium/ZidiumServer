using System;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Настройка пользователя
    /// </summary>
    public class UserSetting
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public virtual User User { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}
