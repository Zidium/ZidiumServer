using System;
using Zidium.Common;
using Zidium.Core;

namespace Zidium.DatabasesUpdate
{
    internal class Configuration : JsonConfiguration<Options>,
        IDatabasesUpdateConfiguration, IDispatcherConfiguration
    {
        public bool UseLocalDispatcher => Get().UseLocalDispatcher;

        public Uri DispatcherUrl => null;

        public IDatabaseConfiguration WorkDatabase => new DatabaseConfiguration(Get().WorkDatabase);

        public IDatabaseConfiguration TestDatabase => new DatabaseConfiguration(Get().TestDatabase);

        public IDatabaseConfiguration LocalDatabase => new DatabaseConfiguration(Get().LocalDatabase);

        public class DatabaseConfiguration : IDatabaseConfiguration
        {
            public DatabaseConfiguration(Options.DatabaseOptions databaseOptions)
            {
                _databaseOptions = databaseOptions;
            }

            private readonly Options.DatabaseOptions _databaseOptions;
            
            public string ProviderName => _databaseOptions.ProviderName;

            public string ConnectionString => _databaseOptions.ConnectionString;

        }

    }
}