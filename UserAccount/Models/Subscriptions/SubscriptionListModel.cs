using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
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
                UserId = UserId,
                Channels = Channels
            };
            var subscriptions = Subscriptions.Where(x => x.Object == obj).ToArray();

            // подписки по умолчанию
            if (obj == SubscriptionObject.Default)
            {
                var row = new SubscriptionsTableRowModel()
                {
                    Cells = new List<ShowSubscriptionCellModel>(),
                    Table = model
                };

                foreach (var channel in Channels)
                {
                    row.Cells.Add(new ShowSubscriptionCellModel()
                    {
                        Channel = channel,
                        Object = SubscriptionObject.Default,
                        Subscription = subscriptions.SingleOrDefault(x => x.Channel == channel && x.Object == SubscriptionObject.Default),
                        Row = row
                    });
                }

                model.Rows = new[] { row };
            }

            // подписки на тип компонента
            if (obj == SubscriptionObject.ComponentType)
            {
                var componentTypeGroups = subscriptions.GroupBy(x => x.ComponentTypeId).Select(x => new
                {
                    ComponentType = x.First().ComponentType,
                    Subscriptions = x.ToArray()
                })
                .OrderBy(x => x.ComponentType.DisplayName)
                .ToArray();

                var rows = new List<SubscriptionsTableRowModel>();
                foreach (var componentTypeGroup in componentTypeGroups)
                {
                    var row = new SubscriptionsTableRowModel()
                    {
                        Cells = new List<ShowSubscriptionCellModel>(),
                        Table = model
                    };

                    foreach (var channel in Channels)
                    {
                        row.Cells.Add(new ShowSubscriptionCellModel()
                        {
                            Channel = channel,
                            Object = SubscriptionObject.ComponentType,
                            ObjectId = componentTypeGroup.ComponentType.Id,
                            Subscription = componentTypeGroup.Subscriptions.SingleOrDefault(x => x.Channel == channel),
                            Row = row
                        });
                    }

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
                        Cells = new List<ShowSubscriptionCellModel>(),
                        Table = model
                    };

                    foreach (var channel in Channels)
                    {
                        row.Cells.Add(new ShowSubscriptionCellModel()
                        {
                            Channel = channel,
                            Object = SubscriptionObject.Component,
                            ObjectId = componentGroup.Component.Id,
                            Subscription = componentGroup.Subscriptions.SingleOrDefault(x => x.Channel == channel),
                            Row = row
                        });
                    }

                    rows.Add(row);
                }
                model.Rows = rows.ToArray();
            }

            return model;
        }

        public SubscriptionChannel[] Channels { get; set; }
    }
}