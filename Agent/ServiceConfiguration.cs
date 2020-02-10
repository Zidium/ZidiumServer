namespace Zidium.Agent
{
    using System;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Reflection;

    public static class ServiceConfiguration
    {
        public static string ConnectionString
        {
            get
            {
                var connectionStringBuilder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConfigDbContext"].ConnectionString);
                return connectionStringBuilder.ConnectionString;
            }
        }

        public static string ServiceDescription
        {
            get
            {
                return GetConfigurationValue("ServiceDescription");
            }
        }

        public static string ServiceName
        {
            get
            {
                return GetConfigurationValue("ServiceName");
            }
        }

        public static int TimeoutInSeconds
        {
            get
            {
                return int.Parse(GetConfigurationValue("TimeoutInSeconds"));
            }
        }

        public static string SmtpServer
        {
            get
            {
                return GetConfigurationValue("SmtpServer");
            }
        }

        public static string SmtpLogin
        {
            get
            {
                return GetConfigurationValue("SmtpLogin");
            }
        }

        public static string SmtpFrom
        {
            get
            {
                return GetConfigurationValue("SmtpFrom");
            }
        }

        public static string SmtpPassword
        {
            get
            {
                return GetConfigurationValue("SmtpPassword");
            }
        }

        public static bool SmtpUseMailKit
        {
            get
            {
                var val = GetConfigurationValue("SmtpUseMailKit");
                return string.Equals("true", val, StringComparison.OrdinalIgnoreCase);
            }
        }

        public static bool SmtpUseSsl
        {
            get
            {
                var val = GetConfigurationValue("SmtpUseSsl");
                return string.Equals("true", val, StringComparison.OrdinalIgnoreCase);
            }
        }

        public static int SmtpPort
        {
            get
            {
                var val = GetConfigurationValue("SmtpPort");
                return int.Parse(val);
            }
        }

        public static string ApiUri
        {
            get { return GetConfigurationValue("ApiUri"); }
        }

        public static string SmsRuApiId
        {
            get
            {
                return GetConfigurationValue("SmsRuApiId");
            }
        }

        public static string SmsRuFrom
        {
            get
            {
                return GetConfigurationValue("SmsRuFrom");
            }
        }

        public static bool DummyMode
        {
            get
            {
                var val = GetConfigurationValue("DummyMode", false);
                return string.Equals("true", val, StringComparison.OrdinalIgnoreCase);
            }
        }

        public static TimeSpan MaximumOfflineInterval
        {
            get
            {
                var val = GetConfigurationValue("MaximumOfflineInterval");
                return TimeSpan.Parse(val);
            }
        }

        public static string TelegramBotToken
        {
            get
            {
                return GetConfigurationValue("TelegramBotToken");
            }
        }

        public static string VKontakteAuthToken
        {
            get
            {
                return GetConfigurationValue("VKontakteAuthToken");
            }
        }

        private static string GetConfigurationValue(string key, bool required = true)
        {
            var service = Assembly.GetEntryAssembly();
            var config = ConfigurationManager.OpenExeConfiguration(service.Location);
            if (config.AppSettings.Settings[key] != null && !string.IsNullOrEmpty(config.AppSettings.Settings[key].Value))
            {
                return config.AppSettings.Settings[key].Value;
            }
            else if (!required)
                return string.Empty;
            else
            {
                var message = string.Format(
                    "В настройках не найдено значение параметра {0} или оно пусто. Настройки искались в папке с файлом {1}.", key, service.Location);
                throw new NullReferenceException(message);
            }
        }
    }
}