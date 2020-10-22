namespace Zidium.Core.Tests
{
    public class Options
    {
        public DatabaseOptions Database;

        public bool DebugMode;

        public bool UseLocalDispatcher;

        public string DispatcherUrl;

        public string ApiUrl;

        public string VirusTotalKey;

        public class DatabaseOptions
        {
            public string ProviderName;

            public string ConnectionString;
        }
    }
}