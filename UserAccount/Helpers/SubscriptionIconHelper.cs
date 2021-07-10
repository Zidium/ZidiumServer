using System;
using Microsoft.AspNetCore.Html;
using Zidium.Api.Dto;
using Zidium.Storage;

namespace Zidium.UserAccount.Helpers
{
    public static class SubscriptionIconHelper
    {
        public static string GetImageUrl(SubscriptionForRead subscription)
        {
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }
            if (subscription.IsEnabled == false)
            {
                return "/content/icons/forbiddance-20px.png";
            }
            if (subscription.Importance == EventImportance.Alarm)
            {
                return "/content/icons/Circle-Red.png";
            }
            if (subscription.Importance == EventImportance.Warning)
            {
                return "/content/icons/Circle-Yellow.png";
            }
            if (subscription.Importance == EventImportance.Success)
            {
                return "/content/icons/Circle-Green.png";
            }
            if (subscription.Importance == EventImportance.Unknown)
            {
                return "/content/icons/Circle-Gray.png";
            }
            return "";
        }

        public static string GetComment(SubscriptionForRead subscription)
        {
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }
            if (subscription.IsEnabled == false)
            {
                return "Запрещено отправлять уведомления";
            }
            if (subscription.Importance == EventImportance.Alarm)
            {
                return "Отправлять уведомления, когда статус будет Alarm";
            }
            if (subscription.Importance == EventImportance.Warning)
            {
                return "Отправлять уведомления, когда статус будет Warning и выше";
            }
            if (subscription.Importance == EventImportance.Success)
            {
                return "Отправлять уведомления, когда статус будет Success и выше";
            }
            if (subscription.Importance == EventImportance.Unknown)
            {
                return "Отправлять уведомления при любом изменении статуса";
            }
            return "";
        }

        public static string GetShortComment(SubscriptionForRead subscription)
        {
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }
            if (subscription.IsEnabled == false)
            {
                return "не отправлять";
            }
            if (subscription.Importance == EventImportance.Alarm)
            {
                return "Alarm";
            }
            if (subscription.Importance == EventImportance.Warning)
            {
                return "Warning";
            }
            if (subscription.Importance == EventImportance.Success)
            {
                return "Success";
            }
            if (subscription.Importance == EventImportance.Unknown)
            {
                return "любой статус";
            }
            return "";
        }

        public static IHtmlContent GetImgHtml(SubscriptionForRead subscription)
        {
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }
            var src = GetImageUrl(subscription);
            var title = GetComment(subscription);
            var html = $"<img class='img-status-icon' src='{src}' title='{title}' />";
            return new HtmlString(html);
        }
    }
}