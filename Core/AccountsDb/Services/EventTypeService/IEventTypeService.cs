using System;
using Zidium.Core.Api;

namespace Zidium.Core.AccountsDb
{
    public interface IEventTypeService
    {
        EventType GetOneById(Guid eventTypeId, Guid accountId);

        EventType GetOrCreate(EventType eventType, Guid accountId);

        EventType GetOneBySystemName(Guid accountId, string systemName);

        void Update(Guid accountId, UpdateEventTypeRequestData data);
    }
}
