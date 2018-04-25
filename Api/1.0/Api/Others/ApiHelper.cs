using System;
using System.Configuration;

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
            // заглушка для юнит-тестов
            var apiUrlFake = ConfigurationManager.AppSettings["ApiUrl"];
            if (string.IsNullOrEmpty(apiUrlFake) == false)
            {
                apiUrlFake = apiUrlFake.Replace("*", accountName);
                return new Uri(apiUrlFake);
            }

            var webVersion = GetWebServiceVersion();
            return new Uri("http://" + accountName + ".api.zidium.net/" + webVersion);
        }
    }
}
