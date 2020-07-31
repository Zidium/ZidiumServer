using System;
using Zidium.Core.ConfigDb;
using Zidium.Storage;

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
                    var configDbServicesFactory = DependencyInjection.GetServicePersistent<IConfigDbServicesFactory>();
                    _accountWebSite = configDbServicesFactory.GetSettingService().GetAccountWebSite();
                }
                return _accountWebSite;
            }
        }

        public static string GetEventUrl(Guid eventId, string accountName, string accountWebSiteUrl = null)
        {
            return GetFullUrl(accountName, string.Format(@"/Events/{0}", eventId), accountWebSiteUrl);
        }

        public static string GetComponentUrl(Guid id, string accountName, string accountWebSiteUrl = null)
        {
            return GetFullUrl(accountName, string.Format(@"/Components/{0}", id), accountWebSiteUrl);
        }

        public static string GetUnitTestUrl(Guid id, string accountName, string accountWebSiteUrl = null)
        {
            return GetFullUrl(accountName, string.Format(@"/UnitTests/ResultDetails/{0}", id), accountWebSiteUrl);
        }

        public static string GetMetricUrl(Guid id, string accountName, string accountWebSiteUrl = null)
        {
            var url = "/Metrics/Show/" + id;
            return GetFullUrl(accountName, url, accountWebSiteUrl);
        }

        public static string GetSubscriptionEditUrl(Guid componentId, Guid userId, string accountName, string accountWebSiteUrl = null)
        {
            var url = $"/Subscriptions/EditComponentSubscriptions?componentId={componentId}&userId={userId}";
            return GetFullUrl(accountName, url, accountWebSiteUrl);
        }

        public static string GetPasswordSetUrl(Guid accountId, Guid token, string accountName, string accountWebSiteUrl = null)
        {
            var url = $"/Home/SetPassword/{token}?accountId={accountId}";
            return GetFullUrl(accountName, url, accountWebSiteUrl);
        }

        public static string GetLoginUrl(string accountName, string accountWebSiteUrl = null)
        {
            var url = "/";
            return GetFullUrl(accountName, url, accountWebSiteUrl);
        }

        public static string GetFullUrl(string accountName, string pathAndQuery, string accountWebSiteUrl = null, string scheme = null)
        {
            var configDbServicesFactory = DependencyInjection.GetServicePersistent<IConfigDbServicesFactory>();
            return configDbServicesFactory.GetUrlService().GetFullUrl(accountName, pathAndQuery, accountWebSiteUrl, scheme);
        }

        public static string GetAccountWebsiteUrl(string accountName, string pathAndQuery, string currentUrl, string accountWebSiteUrl = null)
        {
            var currentUri = new Uri(currentUrl);
            if (!currentUri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
            {
                return GetFullUrl(accountName, pathAndQuery, accountWebSiteUrl, currentUri.Scheme);
            }
            else
            {
                return currentUri.Scheme + "://" + currentUri.Authority + pathAndQuery;
            }
        }

    }
}
