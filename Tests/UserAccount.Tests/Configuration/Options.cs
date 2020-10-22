namespace Zidium.UserAccount.Tests
{
    public class Options
    {
        public DatabaseOptions Database;

        public bool DebugMode;

        public bool UseLocalDispatcher;

        public string DispatcherUrl;

        public string ApiUrl;

        public class DatabaseOptions
        {
            public string ProviderName;

            public string ConnectionString;
        }
    }
}