namespace Zidium.Dispatcher
{
    public class Options
    {
        public DatabaseOptions Database { get; set; }

        public bool DebugMode { get; set; }

        public string WebSite { get; set; }

        public string SecretKey { get; set; }

        public int EventsMaxDays { get; set; }

        public int LogMaxDays { get; set; }

        public int MetricsMaxDays { get; set; }

        public int UnitTestsMaxDays { get; set; }

        public class DatabaseOptions
        {
            public string ProviderName { get; set; }

            public string ConnectionString { get; set; }
        }
    }
}