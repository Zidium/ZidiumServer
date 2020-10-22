namespace Zidium.UserAccount
{
    public class Options
    {
        public DatabaseOptions Database;

        public bool DebugMode;

        public bool UseLocalDispatcher;

        public string DispatcherUrl;

        public class DatabaseOptions
        {
            public string ProviderName;

            public string ConnectionString;
        }
    }
}