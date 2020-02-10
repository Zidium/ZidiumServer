using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;

namespace Zidium.UserAccount.Models.Subscriptions
{
    public class ShowComponentSubscriptionsModel
    {
        public enum ViewModeCode
        {
            My,
            All
        }

        public Guid ComponentId { get; set; }

        public Subscription[] Subscriptions { get; set; }

        public Guid? UserId { get; set; }

        public Guid CurrentUserId { get; set; }

        public SubscriptionChannel? Channel { get; set; }

        public ViewModeCode ViewMode { get; set; }

        public Subscription GetSubscription(Guid userId, SubscriptionChannel channel)
        {
            return Subscriptions.FirstOrDefault(x => x.UserId == userId && x.Channel == channel);
        }

        public void Init()
        {
            Channels = SubscriptionHelper.AvailableSubscriptionChannels;

            var accountDbContext = FullRequestContext.Current.AccountDbContext;
            CurrentUserId = FullRequestContext.Current.CurrentUser.Id;

            var subscriptionsQuery = accountDbContext
                .GetSubscriptionRepository()
                .QueryAll()
                .Where(x => x.User.InArchive == false);

            if (Channel.HasValue)
            {
                subscriptionsQuery = subscriptionsQuery.Where(x => x.Channel == Channel);
            }
            if (UserId.HasValue)
            {
                subscriptionsQuery = subscriptionsQuery.Where(x => x.UserId == UserId);
            }

            var component = accountDbContext.GetComponentRepository().GetById(ComponentId);
            var subscriptions = subscriptionsQuery.ToArray();
            var filtered = new List<Subscription>();
            var userGroups = subscriptions.GroupBy(x => x.UserId);

            var channels = SubscriptionHelper.AvailableSubscriptionChannels;

            foreach (var userGroup in userGroups)
            {
                foreach (var channel in channels)
                {
                    // подписка на компонент
                    var userSubscriptions = userGroup.Where(x => x.Channel == channel).ToArray();

                    var onComponent = userSubscriptions.SingleOrDefault(x =>
                        x.Object == SubscriptionObject.Component
                        && x.ComponentId == ComponentId);

                    if (onComponent != null)
                    {
                        filtered.Add(onComponent);
                        continue;
                    }

                    // подписка на тип компонента
                    var onComponentType = userSubscriptions.SingleOrDefault(x =>
                        x.Object == SubscriptionObject.ComponentType
                        && x.ComponentTypeId == component.ComponentTypeId);

                    if (onComponentType != null)
                    {
                        filtered.Add(onComponentType);
                        continue;
                    }

                    // подписка по умолчанию
                    var defaultSubscription = userSubscriptions.SingleOrDefault(x => x.Object == SubscriptionObject.Default);
                    if (defaultSubscription != null)
                    {
                        filtered.Add(defaultSubscription);
                    }
                }
            }

            Subscriptions = filtered
                .OrderBy(x => x.UserId)
                .ThenBy(x => x.User.LastName)
                .ThenBy(x => x.Channel)
                .ToArray();
        }

        public SubscriptionChannel[] Channels { get; set; }
    }
}