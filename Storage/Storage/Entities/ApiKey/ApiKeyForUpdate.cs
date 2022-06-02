using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Ключ доступа к Api
    /// </summary>
    public class ApiKeyForUpdate
    {
        public ApiKeyForUpdate(Guid id)
        {
            Id = id;
            Name = new ChangeTracker<string>();
            Value = new ChangeTracker<string>();
            UpdatedAt = new ChangeTracker<DateTime>();
            UserId = new ChangeTracker<Guid?>();
        }

        public Guid Id { get; }

        public ChangeTracker<string> Name { get; }

        public ChangeTracker<string> Value { get; }

        public ChangeTracker<DateTime> UpdatedAt { get; }

        /// <summary>
        /// Пользователь, если ключ не общий
        /// </summary>
        public ChangeTracker<Guid?> UserId { get; }
    }
}
