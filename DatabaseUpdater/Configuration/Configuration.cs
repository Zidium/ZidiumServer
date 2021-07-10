using System;
using Microsoft.Extensions.Configuration;
using Zidium.Common;
using Zidium.Core;

namespace Zidium.DatabaseUpdater
{
    internal class Configuration : BaseConfiguration<Options>,
        IDatabasesUpdateConfiguration, IDispatcherConfiguration
    {
        public Configuration(IConfiguration configuration) : base(configuration)
        {
        }

        public bool UseLocalDispatcher => true;

        public Uri DispatcherUrl => null;

        public IDatabaseConfiguration WorkDatabase => new DatabaseConfiguration(Get().WorkDatabase);

        public IDatabaseConfiguration TestDatabase => new DatabaseConfiguration(Get().TestDatabase);

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