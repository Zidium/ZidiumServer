using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Storage;

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

        public SubscriptionInfo[] Subscriptions { get; set; }

        public Guid? UserId { get; set; }

        public Guid CurrentUserId { get; set; }

        public SubscriptionChannel? Channel { get; set; }

        public ViewModeCode ViewMode { get; set; }

        public SubscriptionInfo GetSubscription(Guid userId, SubscriptionChannel channel)
        {
            return Subscriptions.FirstOrDefault(x => x.Info.UserId == userId && x.Info.Channel == channel);
        }

        public SubscriptionChannel[] Channels { get; set; }

        // TODO Move to controller
        public void Init(IStorage storage)
        {
            Channels = SubscriptionHelper.AvailableSubscriptionChannels;

            CurrentUserId = FullRequestContext.Current.CurrentUser.Id;

            var channelsForFilter = Channel.HasValue ? new[] {Channel.Value} : null;
            var subscriptions = storage.Subscriptions.Filter(UserId, channelsForFilter);

            var component = storage.Components.GetOneById(ComponentId);
            var filtered = new List<SubscriptionInfo>();
            var userGroups = subscriptions.GroupBy(x => x.UserId);

            var channels = SubscriptionHelper.AvailableSubscriptionChannels;

            foreach (var userGroup in userGroups)
            {
                var user = storage.Users.GetOneById(userGroup.Key);

                if (user.InArchive)
                    continue;

                foreach (var channel in channels)
                {
                    // подписка на компонент
                    var userSubscriptions = userGroup.Where(x => x.Channel == channel).ToArray();

                    var onComponent = userSubscriptions.SingleOrDefault(x =>
                        x.Object == SubscriptionObject.Component
                        && x.ComponentId == ComponentId);

                    if (onComponent != null)
                    {
                        filtered.Add(new SubscriptionInfo()
                        {
                            Info = onComponent,
                            UserName = user.FioOrLogin()
                        });
                        continue;
                    }

                    // подписка на тип компонента
                    var onComponentType = userSubscriptions.SingleOrDefault(x =>
                        x.Object == SubscriptionObject.ComponentType
                        && x.ComponentTypeId == component.ComponentTypeId);

                    if (onComponentType != null)
                    {
                        filtered.Add(new SubscriptionInfo()
                        {
                            Info = onComponentType,
                            UserName = user.FioOrLogin()
                        });
                        continue;
                    }

                    // подписка по умолчанию
                    var defaultSubscription = userSubscriptions.SingleOrDefault(x => x.Object == SubscriptionObject.Default);
                    if (defaultSubscription != null)
                    {
                        filtered.Add(new SubscriptionInfo()
                        {
                            Info = defaultSubscription,
                            UserName = user.FioOrLogin()
                        });
                    }
                }
            }

            Subscriptions = filtered
                .OrderBy(x => x.UserName)
                .ThenBy(x => x.Info.Channel)
                .ToArray();
        }

        public class SubscriptionInfo
        {
            public SubscriptionForRead Info;

            public string UserName;
        }

    }
}