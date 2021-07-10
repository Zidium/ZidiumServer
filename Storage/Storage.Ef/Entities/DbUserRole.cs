using System;

namespace Zidium.Storage.Ef
{
    public class DbUserRole
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public virtual DbUser User { get; set; }

        public Guid RoleId { get; set; }

        public virtual DbRole Role { get; set; }
    }
}
