using System;
using System.Linq;
using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Models.Subscriptions
{
    public class EditComponentSubscriptionsModel
    {
        public Component Component { get; set; }

        public User User { get; set; }

        public Subscription[] AllSubscriptions { get; set; }

        public class SubscriptionColumnData
        {
            public Subscription Subscription { get; set; }

            public bool Selected { get; set; }

            public SubscriptionChannel Channel { get; set; }

            public SubscriptionObject SubscriptionObject { get; set; }

            public string Title { get; set; }

            public string CreateUrl { get; set; }
        }

        public SubscriptionColumnData GetColumnData(SubscriptionObject subscriptionObject, SubscriptionChannel channel)
        {
            var column = new SubscriptionColumnData()
            {
                Channel = channel,
                SubscriptionObject = subscriptionObject
            };

            // находим подписку
            var channelSubscriptions = AllSubscriptions.Where(x => x.Channel == channel).ToArray();
            if (subscriptionObject == SubscriptionObject.Default)
            {
                column.Subscription = channelSubscriptions.SingleOrDefault(x => x.Object == SubscriptionObject.Default);
                column.Selected = column.Subscription!=null && channelSubscriptions.Length == 1; // единственная подписка
                column.Title = "Подписка по умолчанию";
                column.CreateUrl = $"/Subscriptions/Add?userId={User.Id}&object={subscriptionObject}&channel={channel}";
            }
            else if (subscriptionObject == SubscriptionObject.ComponentType)
            {
                column.Subscription = channelSubscriptions.SingleOrDefault(x =>
                    x.Object == SubscriptionObject.ComponentType
                    && x.ComponentTypeId == Component.ComponentTypeId);

                // если нет подписки на компонент
                column.Selected = column.Subscription != null && channelSubscriptions.FirstOrDefault(x => x.Object == SubscriptionObject.Component) == null;
                column.Title = "Подписка на тип компонента";
                column.CreateUrl = $"/Subscriptions/Add?userId={User.Id}&object={subscriptionObject}&channel={channel}&componentTypeId={Component.ComponentTypeId}";
            }
            else if (subscriptionObject == SubscriptionObject.Component)
            {
                column.Subscription = channelSubscriptions.SingleOrDefault(x =>
                    x.Object == SubscriptionObject.Component
                    && x.ComponentId == Component.Id);

                column.Selected = column.Subscription!=null;
                column.Title = "Подписка на компонент";
                column.CreateUrl = $"/Subscriptions/Add?userId={User.Id}&object={subscriptionObject}&channel={channel}&componentId={Component.Id}";
            }
            else
            {
                throw new Exception("Неизвестный тип подписки " + subscriptionObject);
            }
            return column;
        }
    }
}