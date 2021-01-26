using System;

namespace Zidium.Storage
{
    public interface ISubscriptionRepository
    {
        void Add(SubscriptionForAdd entity);

        void Update(SubscriptionForUpdate entity);

        SubscriptionForRead GetOneById(Guid id);

        SubscriptionForRead GetOneOrNullById(Guid id);

        SubscriptionForRead FindOneOrNull(
            Guid userId,
            SubscriptionObject subscriptionObject,
            SubscriptionChannel channel,
            Guid? componentId,
            Guid? componentTypeId
            );

        SubscriptionForRead[] GetAll();

        SubscriptionForRead[] GetByUserId(Guid userId);

        SubscriptionForRead[] Filter(Guid? userId, SubscriptionChannel[] channels);

        void Delete(Guid id);

    }
}
