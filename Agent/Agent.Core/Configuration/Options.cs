namespace Zidium.Agent
{
    public class Options
    {
        public DatabaseOptions Database { get; set; }

        public bool DebugMode { get; set; }

        public bool UseLocalDispatcher { get; set; }

        public string DispatcherUrl { get; set; }

        public bool DummyMode { get; set; }

        public string MaximumOfflineInterval { get; set; }

        public SmtpOptions Smtp { get; set; }

        public SmsOptions Sms { get; set; }

        public string TelegramBotToken { get; set; }

        public string VKontakteAuthToken { get; set; }

        public int? EventsMaxDeleteCount { get; set; }

        public string SecretKey { get; set; }

        public class DatabaseOptions
        {
            public string ProviderName { get; set; }

            public string ConnectionString { get; set; }
        }


        public class SmtpOptions
        {
            public string SmtpServer { get; set; }

            public int SmtpPort { get; set; }

            public string SmtpLogin { get; set; }

            public string SmtpPassword { get; set; }

            public string SmtpFrom { get; set; }

            public string SmtpFromEmail { get; set; }

            public bool SmtpUseMailKit { get; set; }

            public bool SmtpUseSsl { get; set; }

            public string SmtpLocalServerName { get; set; }
        }

        public class SmsOptions
        {
            public string SmsRuApiId { get; set; }

            public string SmsRuFrom { get; set; }
        }

    }
}