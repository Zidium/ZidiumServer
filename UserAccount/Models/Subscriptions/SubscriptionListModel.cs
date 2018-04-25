using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common.Helpers;

namespace Zidium.UserAccount.Models.Subscriptions
{
    public class SubscriptionListModel
    {
        public Guid UserId { get; set; }

        public string Color { get; set; }

        public Subscription[] Subscriptions { get; set; }

        public SubscriptionsTableModel GetTable(SubscriptionObject obj)
        {
            var model = new SubscriptionsTableModel()
            {
                Object = obj,
                UserId = UserId
            };
            var subscriptions = Subscriptions.Where(x => x.Object == obj).ToArray();

            // подписки по умолчанию
            if (obj == SubscriptionObject.Default)
            {
                var emails = subscriptions.Where(x => x.Channel == SubscriptionChannel.Email).ToArray();
                var sms = subscriptions.Where(x => x.Channel == SubscriptionChannel.Sms).ToArray();

                var row = new SubscriptionsTableRowModel()
                {
                    Email = new ShowSubscriptionCellModel()
                    {
                        Channel = SubscriptionChannel.Email,
                        Object = SubscriptionObject.Default
                    },
                    Sms = new ShowSubscriptionCellModel()
                    {
                        Channel = SubscriptionChannel.Sms,
                        Object = SubscriptionObject.Default
                    }
                };

                row.Email.Subscription = emails.SingleOrDefault(x => x.Object == SubscriptionObject.Default);
                row.Sms.Subscription = sms.SingleOrDefault(x => x.Object == SubscriptionObject.Default);
                model.Rows = new[] {row};
            }

            // подписки на тип компонента
            if (obj == SubscriptionObject.ComponentType)
            {
                var componentTypeGroups = subscriptions.GroupBy(x => x.ComponentTypeId).Select(x=>new
                {
                    ComponentType = x.First().ComponentType,
                    Subscriptions = x.ToArray()
                })
                .OrderBy(x=>x.ComponentType.DisplayName)
                .ToArray();

                var rows = new List<SubscriptionsTableRowModel>();
                foreach (var componentTypeGroup in componentTypeGroups)
                {
                    var row = new SubscriptionsTableRowModel()
                    {
                        Email = new ShowSubscriptionCellModel()
                        {
                            Channel = SubscriptionChannel.Email,
                            Object = SubscriptionObject.ComponentType,
                            ObjectId = componentTypeGroup.ComponentType.Id
                        },
                        Sms = new ShowSubscriptionCellModel()
                        {
                            Channel = SubscriptionChannel.Sms,
                            Object = SubscriptionObject.ComponentType,
                            ObjectId = componentTypeGroup.ComponentType.Id
                        }
                    };

                    row.Email.Subscription = componentTypeGroup.Subscriptions.SingleOrDefault(x => x.Channel == SubscriptionChannel.Email);
                    row.Sms.Subscription = componentTypeGroup.Subscriptions.SingleOrDefault(x => x.Channel == SubscriptionChannel.Sms);
                    rows.Add(row);
                }
                model.Rows = rows.ToArray();
            }

            // подписки на компонент
            if (obj == SubscriptionObject.Component)
            {
                var componentGroups = subscriptions.GroupBy(x => x.ComponentId).Select(x => new
                    {
                        Component = x.First().Component,
                        FullName = ComponentHelper.GetComponentPathText(x.First().Component),
                        Subscriptions = x.ToArray()
                    })
                    .OrderBy(x => x.FullName)
                    .ToArray();

                var rows = new List<SubscriptionsTableRowModel>();
                foreach (var componentGroup in componentGroups)
                {
                    var row = new SubscriptionsTableRowModel()
                    {
                        Email = new ShowSubscriptionCellModel()
                        {
                            Channel = SubscriptionChannel.Email,
                            Object = SubscriptionObject.Component,
                            ObjectId = componentGroup.Component.Id
                        },
                        Sms = new ShowSubscriptionCellModel()
                        {
                            Channel = SubscriptionChannel.Sms,
                            Object = SubscriptionObject.Component,
                            ObjectId = componentGroup.Component.Id
                        }
                    };

                    row.Email.Subscription = componentGroup.Subscriptions.SingleOrDefault(x => x.Channel == SubscriptionChannel.Email);
                    row.Sms.Subscription = componentGroup.Subscriptions.SingleOrDefault(x => x.Channel == SubscriptionChannel.Sms);
                    rows.Add(row);
                }
                model.Rows = rows.ToArray();
            }

            // родительские связи
            foreach (var row in model.Rows)
            {
                row.Table = model;
                row.Email.Row = row;
                row.Sms.Row = row;
            }
            return model;
        }
    }
}