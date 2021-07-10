namespace Zidium.Agent.Single.Tests
{
    public class Options
    {
        public DatabaseOptions Database { get; set; }

        public bool DebugMode { get; set; }

        public bool UseLocalDispatcher { get; set; }

        public string DispatcherUrl { get; set; }

        public string SecretKey { get; set; }

        public class DatabaseOptions
        {
            public string ProviderName { get; set; }

            public string ConnectionString { get; set; }
        }
    }
}