using System;

namespace Zidium.Api.Others
{
    public static class ApiHelper
    {
        public static string GetApiVersion()
        {
             return typeof(Client).Assembly.GetName().Version.ToString(); 
        }

        public static string GetWebServiceVersion()
        {
            var version = GetApiVersion();
            var arr = version.Split('.');
            return arr[0] + "." + arr[1];
        }

        public static Uri GetApiUrl(string accountName)
        {
            var webVersion = GetWebServiceVersion();
            return new Uri("https://" + accountName + ".api.zidium.net/" + webVersion);
        }

    }
}
