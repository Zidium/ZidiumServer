using System;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public class SubscriptionService : ISubscriptionService
    {
        public SubscriptionService(IStorage storage)
        {
            _storage = storage;
        }

        private readonly IStorage _storage;

        public SubscriptionForRead CreateSubscription(CreateSubscriptionRequestData requestData)
        {
            if (requestData == null)
            {
                throw new ParameterRequiredException("Request.Subscription");
            }
            if (requestData.Object == SubscriptionObject.Component && requestData.ComponentId == null)
            {
                throw new ParameterRequiredException("Request.ComponentId");
            }
            if (requestData.Object == SubscriptionObject.ComponentType && requestData.ComponentTypeId == null)
            {
                throw new ParameterRequiredException("Request.ComponentTypeId");
            }
            if (requestData.Object == SubscriptionObject.Default)
            {
                if (requestData.ComponentId.HasValue)
                {
                    throw new UserFriendlyException("Нельзя указывать ComponentId для подписки по умолчанию");
                }
                if (requestData.ComponentTypeId.HasValue)
                {
                    throw new UserFriendlyException("Нельзя указывать ComponentTypeId для подписки по умолчанию");
                }
            }
            else if (requestData.Object == SubscriptionObject.Component)
            {
                if (requestData.ComponentId == null)
                {
                    throw new UserFriendlyException("Укажите ComponentId для подписки на компонент");
                }
                if (requestData.ComponentTypeId.HasValue)
                {
                    throw new UserFriendlyException("Нельзя указывать ComponentTypeId для подписки на компонент");
                }
            }
            else if (requestData.Object == SubscriptionObject.ComponentType)
            {
                if (requestData.ComponentTypeId == null)
                {
                    throw new UserFriendlyException("Укажите ComponentTypeId для подписки на тип компонента");
                }
                if (requestData.ComponentId.HasValue)
                {
                    throw new UserFriendlyException("Нельзя указывать ComponentId для подписки на тип компонента");
                }
            }
            var lockObj = LockObject.ForAccount();
            lock (lockObj)
            {
                var subscription = _storage.Subscriptions.FindOneOrNull(
                    requestData.UserId,
                    requestData.Object,
                    requestData.Channel,
                    requestData.Object == SubscriptionObject.Component ? requestData.ComponentId.Value : (Guid?) null,
                    requestData.Object == SubscriptionObject.ComponentType ? requestData.ComponentTypeId.Value : (Guid?) null
                    );

                if (subscription != null)
                {
                    // ВАЖНО!
                    // Если подписка уже есть, не трогаем её

                    return subscription;
                }

                var user = _storage.Users.GetOneById(requestData.UserId);

                ComponentTypeForRead componentType = null;
                if (requestData.ComponentTypeId.HasValue)
                {
                    componentType = _storage.ComponentTypes.GetOneById(requestData.ComponentTypeId.Value);
                }

                var subscriptionForAdd = new SubscriptionForAdd()
                {
                    Id = Ulid.NewUlid(),
                    UserId = user.Id,
                    Object = requestData.Object,
                    ComponentId = requestData.ComponentId,
                    ComponentTypeId = componentType?.Id,
                    Channel = requestData.Channel,
                    IsEnabled = requestData.IsEnabled,
                    Importance = requestData.Importance,
                    DurationMinimumInSeconds = requestData.DurationMinimumInSeconds,
                    ResendTimeInSeconds = requestData.ResendTimeInSeconds,
                    LastUpdated = DateTime.Now,
                    NotifyBetterStatus = requestData.NotifyBetterStatus,
                    SendOnlyInInterval = requestData.SendOnlyInInterval,
                    SendIntervalFromHour = requestData.SendIntervalFromHour,
                    SendIntervalFromMinute = requestData.SendIntervalFromMinute,
                    SendIntervalToHour = requestData.SendIntervalToHour,
                    SendIntervalToMinute = requestData.SendIntervalToMinute
                };

                _storage.Subscriptions.Add(subscriptionForAdd);

                return _storage.Subscriptions.GetOneById(subscriptionForAdd.Id);
            }
        }

        public void UpdateSubscription(UpdateSubscriptionRequestData requestData)
        {
            if (requestData == null)
            {
                throw new ParameterRequiredException("Request.Subscription");
            }

            var lockObj = LockObject.ForSubscription(requestData.Id);
            lock (lockObj)
            {
                var subscription = new SubscriptionForUpdate(requestData.Id);

                subscription.Importance.Set(requestData.Importance);
                subscription.IsEnabled.Set(requestData.IsEnabled);
                subscription.NotifyBetterStatus.Set(requestData.NotifyBetterStatus);
                subscription.DurationMinimumInSeconds.Set(requestData.DurationMinimumInSeconds);
                subscription.ResendTimeInSeconds.Set(requestData.ResendTimeInSeconds);
                subscription.SendOnlyInInterval.Set(requestData.SendOnlyInInterval);
                subscription.SendIntervalFromHour.Set(requestData.SendIntervalFromHour);
                subscription.SendIntervalFromMinute.Set(requestData.SendIntervalFromMinute);
                subscription.SendIntervalToHour.Set(requestData.SendIntervalToHour);
                subscription.SendIntervalToMinute.Set(requestData.SendIntervalToMinute);
                subscription.LastUpdated.Set(DateTime.Now);

                _storage.Subscriptions.Update(subscription);
            }
        }

        private void SetSubscriptionEnable(Guid subscriptionId, bool enable)
        {
            var lockObj = LockObject.ForSubscription(subscriptionId);
            lock (lockObj)
            {
                var subscription = new SubscriptionForUpdate(subscriptionId);
                subscription.IsEnabled.Set(enable);
                subscription.LastUpdated.Set(DateTime.Now);
                _storage.Subscriptions.Update(subscription);
            }
        }

        public void SetSubscriptionEnable(SetSubscriptionEnableRequestData requestData)
        {
            if (requestData == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            SetSubscriptionEnable(requestData.Id, true);
        }

        public void SetSubscriptionDisable(SetSubscriptionDisableRequestData requestData)
        {
            if (requestData == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            SetSubscriptionEnable(requestData.Id, false);
        }

        public void DeleteSubscription(DeleteSubscriptionRequestData requestData)
        {
            if (requestData == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            if (requestData.SubscriptionId == null)
            {
                throw new ParameterRequiredException("Request.Data.SubscriptionId");
            }

            var subscriptionId = requestData.SubscriptionId.Value;

            var lockObj = LockObject.ForSubscription(subscriptionId);

            lock (lockObj)
            {
                _storage.Notifications.DeleteBySubscriptionId(subscriptionId);
                _storage.Subscriptions.Delete(subscriptionId);
            }
        }

        public SubscriptionForRead CreateDefaultForUser(Guid userId)
        {
            var lockObj = LockObject.ForAccount();

            lock (lockObj)
            {
                var defaultForUser = new SubscriptionForAdd()
                {
                    Id = Ulid.NewUlid(),
                    UserId = userId,
                    ComponentTypeId = null,
                    Channel = SubscriptionChannel.Email,
                    Object = SubscriptionObject.Default,
                    IsEnabled = true,
                    Importance = EventImportance.Alarm,
                    DurationMinimumInSeconds = 10 * 60,
                    ResendTimeInSeconds = 24 * 60 * 60,
                    NotifyBetterStatus = false,
                    SendOnlyInInterval = false,
                    LastUpdated = DateTime.Now
                };
                _storage.Subscriptions.Add(defaultForUser);

                var forRoot = new SubscriptionForAdd()
                {
                    Id = Ulid.NewUlid(),
                    UserId = userId,
                    Channel = SubscriptionChannel.Email,
                    Object = SubscriptionObject.ComponentType,
                    ComponentTypeId = SystemComponentType.Root.Id,
                    IsEnabled = false,
                    LastUpdated = DateTime.Now
                };
                _storage.Subscriptions.Add(forRoot);

                var forFolder = new SubscriptionForAdd()
                {
                    Id = Ulid.NewUlid(),
                    UserId = userId,
                    Channel = SubscriptionChannel.Email,
                    Object = SubscriptionObject.ComponentType,
                    ComponentTypeId = SystemComponentType.Folder.Id,
                    IsEnabled = false,
                    LastUpdated = DateTime.Now
                };
                _storage.Subscriptions.Add(forFolder);

                return _storage.Subscriptions.GetOneById(defaultForUser.Id);
            }
        }

    }
}
