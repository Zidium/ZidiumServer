using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core.AccountsDb;
using Zidium.Core.AccountsDb.Classes;
using Zidium.Core.Api;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Helpers
{
    public static class DropDownListHelper
    {

        private static AccountDbContext GetAccountDbContext()
        {
            return FullRequestContext.Current.AccountDbContext;
        }

        private static Guid GetAccountId()
        {
            return FullRequestContext.Current.CurrentUser.AccountId;
        }

        public static List<SelectListItem> GetIntegers(int? selected, int[] values, bool allowEmpty = false)
        {
            var items = values.Select(x => new SelectListItem()
            {
                Value = x.ToString(),
                Text = x.ToString(),
                Selected = x == selected
            }).ToList();

            return GetAllowEmptyItems(items, allowEmpty);
        }

        private static List<SelectListItem> GetAllowEmptyItems(List<SelectListItem> items, bool allowEmpty)
        {
            if (allowEmpty)
            {
                var item = new SelectListItem();
                items.Insert(0, item);
            }
            return items;
        }

        public static List<SelectListItem> GetMetricTypes(Guid? selected, bool allowEmpty)
        {
            var repository = FullRequestContext.Current.AccountDbContext.GetMetricTypeRepository();
            var metricTypes = repository.QueryAll();
            var items = metricTypes
                .OrderBy(t => t.DisplayName)
                .Select(x => new SelectListItem()
                {
                    Text = x.DisplayName,
                    Selected = x.Id == selected,
                    Value = x.Id.ToString()
                })
                .ToList();
            return GetAllowEmptyItems(items, allowEmpty);
        }

        public static List<SelectListItem> GetDefectStatuses(DefectStatus? selected, bool allowEmpty = false)
        {
            var all = new[]
            {
                DefectStatus.Open,
                DefectStatus.ReOpen,
                DefectStatus.InProgress,
                DefectStatus.Testing,
                DefectStatus.Closed
            };
            var enumName = FullRequestContext.Current.EnumName;

            var items = all.Select(x => new SelectListItem()
            {
                Value = x.ToString(),
                Text = enumName.GetName(x),
                Selected = x == selected
            }).ToList();

            return GetAllowEmptyItems(items, allowEmpty);
        }

        public static List<SelectListItem> GetCustomEventCategories(EventCategory? selected, bool allowEmpty)
        {
            var categories = new[]
            {
                EventCategory.ApplicationError,
                EventCategory.ComponentEvent
            };
            return categories.Select(x => new SelectListItem()
            {
                Text = x.ToString(),
                Selected = x == selected,
                Value = x.ToString()
            }).ToList();
        }

        public static List<SelectListItem> GetSubscriptionChannels(SubscriptionChannel? selected, bool allowEmpty)
        {
            var channels = new[]
            {
                SubscriptionChannel.Sms,
                SubscriptionChannel.Email
            };
            var items = channels.Select(x => new SelectListItem()
            {
                Text = x.ToString(),
                Selected = x == selected,
                Value = x.ToString()
            }).ToList();

            return GetAllowEmptyItems(items, allowEmpty);
        }

        public static List<SelectListItem> GetUsers(Guid? selected, bool allowEmpty)
        {
            var repository = GetAccountDbContext().GetUserRepository();
            var users = repository.QueryAll().ToArray();

            var items = users.Select(x => new SelectListItem()
            {
                Text = x.DisplayName,
                Value = x.Id.ToString(),
                Selected = x.Id == selected
            }).ToList();

            return GetAllowEmptyItems(items, allowEmpty);
        }

        public static List<SelectListItem> GetComponentTypes(Guid? selected, bool allowEmpty)
        {
            var repository = GetAccountDbContext().GetComponentTypeRepository();
            var accountId = GetAccountId();
            var types = repository.QueryAll().OrderBy(x=>x.DisplayName).ToArray();

            var items = types.Select(x => new SelectListItem()
            {
                Text = x.DisplayName,
                Value = x.Id.ToString(),
                Selected = x.Id == selected
            }).ToList();

            return GetAllowEmptyItems(items, allowEmpty);
        }

        public static List<SelectListItem> GetEventImportances(EventImportance? selected, bool allowEmpty)
        {
            var importances = new[]
            {
                EventImportance.Alarm,
                EventImportance.Warning,
                EventImportance.Success,
                EventImportance.Unknown
            };
            var items = new List<SelectListItem>();
            if (allowEmpty)
            {
                items.Add(new SelectListItem()
                {
                    Value = "",
                    Text = ""
                });
            }
            foreach (var importance in importances)
            {
                var item = new SelectListItem()
                {
                    Value = importance.ToString(),
                    Text = importance.ToString(),
                    Selected = importance == selected
                };
                items.Add(item);
            }
            return items;
        }
    }
}