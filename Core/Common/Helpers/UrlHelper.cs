using System;
using Zidium.Core.Api;

namespace Zidium.Core.Common
{
    /// <summary>
    /// Класс получает адреса страниц
    /// </summary>
    public static class UrlHelper
    {
        private static string _accountWebSite;

        public static string AccountWebSite
        {
            get
            {
                if (_accountWebSite == null)
                {
                    var dispatcherClient = DispatcherHelper.GetDispatcherClient();
                    var logicSettings = dispatcherClient.GetLogicSettings().GetDataAndCheck();
                    _accountWebSite = logicSettings.AccountWebSite;
                }
                return _accountWebSite;
            }
        }

        public static string GetEventUrl(Guid eventId, string accountWebSiteUrl = null)
        {
            return GetFullUrl(string.Format(@"/Events/{0}", eventId), accountWebSiteUrl);
        }

        public static string GetComponentUrl(Guid id, string accountWebSiteUrl = null)
        {
            return GetFullUrl(string.Format(@"/Components/{0}", id), accountWebSiteUrl);
        }

        public static string GetUnitTestUrl(Guid id, string accountWebSiteUrl = null)
        {
            return GetFullUrl(string.Format(@"/UnitTests/ResultDetails/{0}", id), accountWebSiteUrl);
        }

        public static string GetMetricUrl(Guid id, string accountWebSiteUrl = null)
        {
            var url = "/Metrics/Show/" + id;
            return GetFullUrl(url, accountWebSiteUrl);
        }

        public static string GetSubscriptionEditUrl(Guid componentId, Guid userId, string accountWebSiteUrl = null)
        {
            var url = $"/Subscriptions/EditComponentSubscriptions?componentId={componentId}&userId={userId}";
            return GetFullUrl(url, accountWebSiteUrl);
        }

        public static string GetPasswordSetUrl(Guid token, string accountWebSiteUrl = null)
        {
            var url = $"/Home/SetPassword/{token}";
            return GetFullUrl(url, accountWebSiteUrl);
        }

        public static string GetLoginUrl(string accountWebSiteUrl = null)
        {
            var url = "/";
            return GetFullUrl(url, accountWebSiteUrl);
        }

        public static string GetFullUrl(string pathAndQuery, string accountWebSiteUrl = null)
        {
            var uri = new Uri(accountWebSiteUrl ?? AccountWebSite);
            // TODO Use uri builder
            var result = new Uri(uri.Scheme + "://" + uri.Authority + pathAndQuery);
            return result.AbsoluteUri;
        }

        public static string GetAccountWebsiteUrl(string pathAndQuery, string currentUrl, string accountWebSiteUrl = null)
        {
            var currentUri = new Uri(currentUrl);
            if (!currentUri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
            {
                return GetFullUrl(pathAndQuery, accountWebSiteUrl);
            }
            else
            {
                return currentUri.Scheme + "://" + currentUri.Authority + pathAndQuery;
            }
        }

    }
}
