namespace Zidium.DatabaseUpdater
{
    public class Options
    {
        public DatabaseOptions WorkDatabase { get; set; }
        
        public DatabaseOptions TestDatabase { get; set; }

        public DatabaseOptions LocalDatabase { get; set; }

        public class DatabaseOptions
        {
            public string ProviderName { get; set; }

            public string ConnectionString { get; set; }
        }
    }
}