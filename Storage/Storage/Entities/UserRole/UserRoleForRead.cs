using System;

namespace Zidium.Storage
{
    public class UserRoleForRead
    {
        public UserRoleForRead(Guid id, Guid userId, Guid roleId)
        {
            Id = id;
            UserId = userId;
            RoleId = roleId;
        }

        public Guid Id { get; }

        public Guid UserId { get; }

        public Guid RoleId { get; }

    }
}
