using System;

namespace Zidium.Storage
{
    public interface ILastComponentNotificationRepository
    {
        void Add(LastComponentNotificationForAdd entity);

        void Update(LastComponentNotificationForUpdate entity);

        LastComponentNotificationForRead GetOneById(Guid id);

    }
}
