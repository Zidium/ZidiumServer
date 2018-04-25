using System;
using System.Configuration;

namespace Zidium.Core
{
    public static class DebugHelper
    {
        public static bool IsDebugMode
        {
            get
            {
                var value = ConfigurationManager.AppSettings["DebugMode"];
                return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
            }
        }

        public static string DebugComponentType = "DebugModeComponent";
    }
}
