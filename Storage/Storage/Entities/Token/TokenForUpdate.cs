using System;

namespace Zidium.Storage
{
    public class TokenForUpdate
    {
        public TokenForUpdate(Guid id)
        {
            Id = id;
            IsUsed = new ChangeTracker<bool>();
        }

        public Guid Id { get; }

        public ChangeTracker<bool> IsUsed { get; }

    }
}