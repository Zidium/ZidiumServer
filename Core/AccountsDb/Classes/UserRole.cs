using System;

namespace Zidium.Core.AccountsDb
{
    public class UserRole
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public virtual User User { get; set; }

        public Guid RoleId { get; set; }

        public virtual Role Role { get; set; }
    }
}
