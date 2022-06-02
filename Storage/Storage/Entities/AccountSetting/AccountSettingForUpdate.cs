using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Настройка аккаунта
    /// </summary>
    public class AccountSettingForUpdate
    {
        public AccountSettingForUpdate(Guid id)
        {
            Id = id;
            Value = new ChangeTracker<string>();
        }

        public Guid Id { get; }

        public ChangeTracker<string> Value { get; }
    }
}