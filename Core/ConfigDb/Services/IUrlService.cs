namespace Zidium.Core.ConfigDb
{
    public interface IUrlService
    {
        string GetFullUrl(string accountName, string pathAndQuery, string accountWebSiteUrl = null, string scheme = null);

        string GetCommonWebsiteUrl();

        string GetAccountNameFromUrl(string url, string accountWebSiteUrl = null);
    }
}
