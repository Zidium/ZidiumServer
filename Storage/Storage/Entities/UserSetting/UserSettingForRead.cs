using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Настройка пользователя
    /// </summary>
    public class UserSettingForRead
    {
        public UserSettingForRead(Guid id, Guid userId, string name, string value)
        {
            Id = id;
            UserId = userId;
            Name = name;
            Value = value;
        }

        public Guid Id { get; }

        public Guid UserId { get; }

        public string Name { get; }

        public string Value { get; }
    }
}
