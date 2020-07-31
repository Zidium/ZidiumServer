using System;

namespace Zidium.Storage.Ef
{
    public class DbToken
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public virtual DbUser User { get; set; }

        public TokenPurpose Purpose { get; set; }

        public string SecurityStamp { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsUsed { get; set; }
    }
}
