using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Ключ доступа к Api
    /// </summary>
    public class ApiKeyForRead
    {
        public ApiKeyForRead(
            Guid id,
            string name,
            string value,
            DateTime updatedAt,
            Guid? userId)
        {
            Id = id;
            Name = name;
            Value = value;
            UpdatedAt = updatedAt;
            UserId = userId;
        }

        public Guid Id { get; }

        public string Name { get; }

        public string Value { get; }

        public DateTime UpdatedAt { get; }

        /// <summary>
        /// Пользователь, если ключ не общий
        /// </summary>
        public Guid? UserId { get; }

        public ApiKeyForUpdate GetForUpdate()
        {
            return new ApiKeyForUpdate(Id);
        }
    }
}
