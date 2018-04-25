using System;
using System.Configuration;

namespace Zidium.DispatcherHttpService
{
    public static class Config
    {
        private static string GetStringOrNull(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static bool GetBool(string key)
        {
            var value = GetStringOrNull(key);
            return string.Equals(value, "true", StringComparison.InvariantCultureIgnoreCase);
        }

    }
}