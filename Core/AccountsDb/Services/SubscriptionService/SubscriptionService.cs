using System;
using System.Linq;
using Zidium.Core.Api;
using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    public class SubscriptionService : ISubscriptionService
    {
        protected DatabasesContext Context { get; set; }

        public SubscriptionService(DatabasesContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public Subscription CreateSubscription(Guid accountId, CreateSubscriptionRequestData requestData)
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
            var lockObj = LockObject.ForAccount(accountId);
            lock (lockObj)
            {
                var accountDbContext = Context.GetAccountDbContext(accountId);
                var subscriptionRepository = accountDbContext.GetSubscriptionRepository();

                var query = subscriptionRepository
                    .QueryAll()
                    .Where(t =>
                        t.UserId == requestData.UserId
                        && t.Object == requestData.Object
                        && t.Channel == requestData.Channel);

                if (requestData.Object == SubscriptionObject.Component)
                {
                    query = query.Where(x => x.ComponentId == requestData.ComponentId.Value);
                }
                else if (requestData.Object == SubscriptionObject.ComponentType)
                {
                    query = query.Where(x => x.ComponentTypeId == requestData.ComponentTypeId.Value);
                }

                var subscription = query.FirstOrDefault();
                if (subscription != null)
                {
                    // ВАЖНО!
                    // Если подписка уже есть, не трогаем её

                    return subscription;
                }

                var userRepository = accountDbContext.GetUserRepository();
                var user = userRepository.GetById(requestData.UserId);

                ComponentType componentType = null;
                if (requestData.ComponentTypeId.HasValue)
                {
                    var componentTypeRepository = accountDbContext.GetComponentTypeRepository();
                    componentType = componentTypeRepository.GetById(requestData.ComponentTypeId.Value);
                }

                subscription = new Subscription()
                {
                    User = user,
                    Object = requestData.Object,
                    ComponentId = requestData.ComponentId,
                    ComponentType = componentType,
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

                subscription = subscriptionRepository.Add(subscription);
                accountDbContext.SaveChanges();

                return subscription;
            }
        }

        public Subscription UpdateSubscription(Guid accountId, UpdateSubscriptionRequestData requestData)
        {
            if (requestData == null)
            {
                throw new ParameterRequiredException("Request.Subscription");
            }

            var lockObj = LockObject.ForSubscription(requestData.Id);
            lock (lockObj)
            {

                var accountDbContext = Context.GetAccountDbContext(accountId);
                var subsRepository = accountDbContext.GetSubscriptionRepository();

                var subscription = subsRepository.GetById(requestData.Id);

                subscription.Importance = requestData.Importance;
                subscription.IsEnabled = requestData.IsEnabled;
                subscription.NotifyBetterStatus = requestData.NotifyBetterStatus;
                subscription.DurationMinimumInSeconds = requestData.DurationMinimumInSeconds;
                subscription.ResendTimeInSeconds = requestData.ResendTimeInSeconds;
                subscription.SendOnlyInInterval = requestData.SendOnlyInInterval;
                subscription.SendIntervalFromHour = requestData.SendIntervalFromHour;
                subscription.SendIntervalFromMinute = requestData.SendIntervalFromMinute;
                subscription.SendIntervalToHour = requestData.SendIntervalToHour;
                subscription.SendIntervalToMinute = requestData.SendIntervalToMinute;
                subscription.LastUpdated = DateTime.Now;

                accountDbContext.SaveChanges();

                return subscription;
            }
        }

        private Subscription SetSubscriptionEnable(Guid accountId, Guid subscriptionId, bool enable)
        {
            var lockObj = LockObject.ForSubscription(subscriptionId);
            lock (lockObj)
            {
                var accountDbContext = Context.GetAccountDbContext(accountId);
                var subsRepository = accountDbContext.GetSubscriptionRepository();
                var subscription = subsRepository.GetById(subscriptionId);
                subscription.IsEnabled = enable;
                subscription.LastUpdated = DateTime.Now;
                accountDbContext.SaveChanges();
                return subscription;
            }
        }

        public Subscription SetSubscriptionEnable(Guid accountId, SetSubscriptionEnableRequestData requestData)
        {
            if (requestData == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            return SetSubscriptionEnable(accountId, requestData.Id, true);
        }

        public Subscription SetSubscriptionDisable(Guid accountId, SetSubscriptionDisableRequestData requestData)
        {
            if (requestData == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }
            return SetSubscriptionEnable(accountId, requestData.Id, false);
        }

        public void DeleteSubscription(Guid accountId, DeleteSubscriptionRequestData requestData)
        {
            if (requestData == null)
            {
                throw new ParameterRequiredException("Request.Data");
            }

            var lockObj = LockObject.ForSubscription(requestData.SubscriptionId.Value);

            lock (lockObj)
            {
                var accountDbContext = Context.GetAccountDbContext(accountId);
                var subscription = accountDbContext.GetSubscriptionRepository().GetById(requestData.SubscriptionId.Value);

                var notificationRepository = accountDbContext.GetNotificationRepository();
                notificationRepository.DeleteBySubscriptionId(subscription.Id);

                var subscriptionRepository = accountDbContext.GetSubscriptionRepository();
                subscriptionRepository.Remove(subscription);
            }
        }

        public Subscription CreateDefaultForUser(Guid accountId, Guid userId)
        {
            var accountDbContext = Context.GetAccountDbContext(accountId);
            var repository = accountDbContext.GetSubscriptionRepository();

            var lockObj = LockObject.ForAccount(accountId);

            lock (lockObj)
            {
                var defaultForUser = new Subscription()
                {
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
                repository.Add(defaultForUser);

                var forRoot = new Subscription()
                {
                    UserId = userId,
                    Channel = SubscriptionChannel.Email,
                    Object = SubscriptionObject.ComponentType,
                    ComponentTypeId = SystemComponentTypes.Root.Id,
                    IsEnabled = false,
                    LastUpdated = DateTime.Now
                };
                repository.Add(forRoot);

                var forFolder = new Subscription()
                {
                    UserId = userId,
                    Channel = SubscriptionChannel.Email,
                    Object = SubscriptionObject.ComponentType,
                    ComponentTypeId = SystemComponentTypes.Folder.Id,
                    IsEnabled = false,
                    LastUpdated = DateTime.Now
                };
                repository.Add(forFolder);

                return defaultForUser;
            }
        }

    }
}
