using System;

namespace Zidium.Storage
{
    public interface IEventPropertyRepository
    {
        EventPropertyForRead GetOneById(Guid id);

        EventPropertyForRead[] GetByEventId(Guid eventId);

        EventPropertyForRead[] GetByEventIds(Guid[] eventIds);

    }
}
