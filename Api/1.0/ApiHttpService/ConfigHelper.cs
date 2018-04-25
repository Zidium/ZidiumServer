using System;
using System.Configuration;

namespace Zidium.ApiHttpService
{
    public static class ConfigHelper
    {
        private static string GetString(string key)
        {
            string value = GetStringOrNull(key);
            if (value == null)
            {
                throw new Exception("Не удалось найти параметр " + key);
            }
            return value;
        }

        private static string GetStringOrNull(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static Uri GetDispatcherUri()
        {
            var uri = GetString("DispatcherUri");
            return new Uri(uri);
        }
    }
}