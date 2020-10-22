namespace Zidium.Agent
{
    public class Options
    {
        public DatabaseOptions Database;

        public bool DebugMode;

        public bool UseLocalDispatcher;

        public string DispatcherUrl;

        public InstallOptions Install;

        public bool DummyMode;

        public string MaximumOfflineInterval;

        public SmtpOptions Smtp;

        public SmsOptions Sms;

        public string TelegramBotToken;

        public string VKontakteAuthToken;

        public int? EventsMaxDeleteCount;

        public class DatabaseOptions
        {
            public string ProviderName;

            public string ConnectionString;
        }

        public class InstallOptions
        {
            public string ServiceName;

            public string ServiceDescription;
        }

        public class SmtpOptions
        {
            public string SmtpServer;

            public int SmtpPort;

            public string SmtpLogin;

            public string SmtpPassword;

            public string SmtpFrom;

            public bool SmtpUseMailKit;

            public bool SmtpUseSsl;
        }

        public class SmsOptions
        {
            public string SmsRuApiId;

            public string SmsRuFrom;
        }

    }
}