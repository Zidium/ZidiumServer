using System;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    public interface IEventTypeRepository : IAccountBasedRepository<EventType>
    {
        EventType GetOneOrNullBySystemName(string systemName);

        EventType GetOrCreate(EventType eventType);

        IQueryable<EventType> QueryAllWithDeleted();
    }
}
