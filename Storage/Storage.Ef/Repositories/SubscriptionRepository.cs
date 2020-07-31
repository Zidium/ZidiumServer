using System;
using System.Linq;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class SubscriptionRepository : ISubscriptionRepository
    {
        public SubscriptionRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(SubscriptionForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.Subscriptions.Add(new DbSubscription()
                {
                    Id = entity.Id,
                    ComponentId = entity.ComponentId,
                    ComponentTypeId = entity.ComponentTypeId,
                    Channel = entity.Channel,
                    DurationMinimumInSeconds = entity.DurationMinimumInSeconds,
                    Importance = entity.Importance,
                    IsEnabled = entity.IsEnabled,
                    LastUpdated = entity.LastUpdated,
                    NotifyBetterStatus = entity.NotifyBetterStatus,
                    Object = entity.Object,
                    ResendTimeInSeconds = entity.ResendTimeInSeconds,
                    SendIntervalFromHour = entity.SendIntervalFromHour,
                    SendIntervalFromMinute = entity.SendIntervalFromMinute,
                    SendIntervalToHour = entity.SendIntervalToHour,
                    SendIntervalToMinute = entity.SendIntervalToMinute,
                    SendOnlyInInterval = entity.SendOnlyInInterval,
                    UserId = entity.UserId
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(SubscriptionForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var subscription = DbGetOneById(entity.Id);

                if (entity.IsEnabled.Changed())
                    subscription.IsEnabled = entity.IsEnabled.Get();

                if (entity.NotifyBetterStatus.Changed())
                    subscription.NotifyBetterStatus = entity.NotifyBetterStatus.Get();

                if (entity.Importance.Changed())
                    subscription.Importance = entity.Importance.Get();

                if (entity.DurationMinimumInSeconds.Changed())
                    subscription.DurationMinimumInSeconds = entity.DurationMinimumInSeconds.Get();

                if (entity.ResendTimeInSeconds.Changed())
                    subscription.ResendTimeInSeconds = entity.ResendTimeInSeconds.Get();

                if (entity.LastUpdated.Changed())
                    subscription.LastUpdated = entity.LastUpdated.Get();

                if (entity.SendOnlyInInterval.Changed())
                    subscription.SendOnlyInInterval = entity.SendOnlyInInterval.Get();

                if (entity.SendIntervalFromHour.Changed())
                    subscription.SendIntervalFromHour = entity.SendIntervalFromHour.Get();

                if (entity.SendIntervalFromMinute.Changed())
                    subscription.SendIntervalFromMinute = entity.SendIntervalFromMinute.Get();

                if (entity.SendIntervalToHour.Changed())
                    subscription.SendIntervalToHour = entity.SendIntervalToHour.Get();

                if (entity.SendIntervalToMinute.Changed())
                    subscription.SendIntervalToMinute = entity.SendIntervalToMinute.Get();

                contextWrapper.Context.SaveChanges();
            }
        }

        public SubscriptionForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public SubscriptionForRead FindOneOrNull(
            Guid userId,
            SubscriptionObject subscriptionObject,
            SubscriptionChannel channel,
            Guid? componentId,
            Guid? componentTypeId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var query = contextWrapper.Context.Subscriptions.AsNoTracking()
                    .Where(t => t.UserId == userId &&
                                t.Object == subscriptionObject &&
                                t.Channel == channel);

                if (componentId.HasValue)
                    query = query.Where(t => t.ComponentId == componentId.Value);

                if (componentTypeId.HasValue)
                    query = query.Where(t => t.ComponentTypeId == componentTypeId.Value);

                return DbToEntity(query.FirstOrDefault());
            }
        }

        public SubscriptionForRead[] GetAll()
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Subscriptions.AsNoTracking()
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public SubscriptionForRead[] GetByUserId(Guid userId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Subscriptions.AsNoTracking()
                    .Where(t => t.UserId == userId)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public SubscriptionForRead[] Filter(Guid? userId, SubscriptionChannel[] channels)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var query = contextWrapper.Context.Subscriptions.AsNoTracking().AsQueryable();

                if (userId.HasValue)
                    query = query.Where(t => t.UserId == userId.Value);

                if (channels != null && channels.Length > 0)
                    query = query.Where(t => channels.Contains(t.Channel));

                return query
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public void Delete(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var subscription = DbGetOneById(id);
                contextWrapper.Context.Subscriptions.Remove(subscription);
                contextWrapper.Context.SaveChanges();
            }
        }

        private DbSubscription DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Subscriptions.Find(id);
            }
        }

        private DbSubscription DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Подписка {id} не найдена");

            return result;
        }

        private SubscriptionForRead DbToEntity(DbSubscription entity)
        {
            if (entity == null)
                return null;

            return new SubscriptionForRead(entity.Id, entity.UserId, entity.ComponentTypeId, entity.ComponentId,
                entity.Object, entity.Channel, entity.IsEnabled, entity.NotifyBetterStatus, entity.Importance,
                entity.DurationMinimumInSeconds, entity.ResendTimeInSeconds, entity.LastUpdated, entity.SendOnlyInInterval,
                entity.SendIntervalFromHour, entity.SendIntervalFromMinute, entity.SendIntervalToHour, entity.SendIntervalToMinute);
        }
    }
}
