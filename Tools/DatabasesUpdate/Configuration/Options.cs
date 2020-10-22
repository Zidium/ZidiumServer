namespace Zidium.DatabasesUpdate
{
    public class Options
    {
        public DatabaseOptions WorkDatabase;
        
        public DatabaseOptions TestDatabase;
        
        public DatabaseOptions LocalDatabase;

        public bool UseLocalDispatcher;

        public class DatabaseOptions
        {
            public string ProviderName;

            public string ConnectionString;
        }
    }
}