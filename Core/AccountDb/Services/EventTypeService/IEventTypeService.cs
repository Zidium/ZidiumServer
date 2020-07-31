using System;
using Zidium.Core.Api;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public interface IEventTypeService
    {
        // TODO Remove trivial method
        EventTypeForRead GetOneById(Guid eventTypeId, Guid accountId);

        EventTypeForRead GetOrCreate(EventTypeForAdd eventType, Guid accountId);

        // TODO Remove trivial method
        EventTypeForRead GetOneBySystemName(Guid accountId, string systemName);

        void Update(Guid accountId, UpdateEventTypeRequestData data);
    }
}
