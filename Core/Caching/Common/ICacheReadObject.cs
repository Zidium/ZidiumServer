using System;

namespace Zidium.Core.Caching
{
    public interface ICacheReadObject
    {
        Guid Id { get; }

        void WaitSaveChanges();

        void WaitSaveChanges(TimeSpan timeOut);
    }
}
