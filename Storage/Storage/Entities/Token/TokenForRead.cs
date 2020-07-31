using System;

namespace Zidium.Storage
{
    public class TokenForRead
    {
        public TokenForRead(
            Guid id, 
            Guid userId, 
            TokenPurpose purpose, 
            string securityStamp, 
            DateTime creationDate, 
            DateTime endDate, 
            bool isUsed)
        {
            Id = id;
            UserId = userId;
            Purpose = purpose;
            SecurityStamp = securityStamp;
            CreationDate = creationDate;
            EndDate = endDate;
            IsUsed = isUsed;
        }

        public Guid Id { get; }

        public Guid UserId { get; }

        public TokenPurpose Purpose { get; }

        public string SecurityStamp { get; }

        public DateTime CreationDate { get; }

        public DateTime EndDate { get; }

        public bool IsUsed { get; }

        public TokenForUpdate GetForUpdate()
        {
            return new TokenForUpdate(Id);
        }
    }
}
