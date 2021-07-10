using System;
using Zidium.Core.Api;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public interface IEventTypeService
    {
        // TODO Remove trivial method
        EventTypeForRead GetOneById(Guid eventTypeId);

        EventTypeForRead GetOrCreate(EventTypeForAdd eventType);

        // TODO Remove trivial method
        EventTypeForRead GetOneBySystemName(string systemName);

        void Update(UpdateEventTypeRequestData data);
    }
}
