using System;

namespace Zidium.Storage
{
    public class TokenForAdd
    {
        public Guid Id;

        public Guid UserId;

        public TokenPurpose Purpose;

        public string SecurityStamp;

        public DateTime CreationDate;

        public DateTime EndDate;

        public bool IsUsed;
    }
}
