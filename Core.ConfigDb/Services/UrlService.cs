using System;

namespace Zidium.Core.ConfigDb
{
    public class UrlService : IUrlService
    {
        public string GetFullUrl(string accountName, string pathAndQuery, string accountWebSiteUrl = null, string scheme = null)
        {
            var uri = new Uri(accountWebSiteUrl ?? Common.UrlHelper.AccountWebSite);
            var result = new Uri((scheme ?? uri.Scheme) + "://" + uri.Authority + pathAndQuery);
            return result.AbsoluteUri;
        }

        public string GetCommonWebsiteUrl()
        {
            return "https://zidium.net";
        }

        public string GetAccountNameFromUrl(string url, string accountWebSiteUrl = null)
        {
            return null;
        }
    }
}
