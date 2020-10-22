namespace Zidium.DispatcherHttpService
{
    public class Options
    {
        public DatabaseOptions Database;

        public bool DebugMode;

        public class DatabaseOptions
        {
            public string ProviderName;

            public string ConnectionString;
        }
    }
}