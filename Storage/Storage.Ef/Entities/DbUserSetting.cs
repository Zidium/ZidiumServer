using System;

namespace Zidium.Storage.Ef
{
    /// <summary>
    /// Настройка пользователя
    /// </summary>
    public class DbUserSetting
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public virtual DbUser User { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}
