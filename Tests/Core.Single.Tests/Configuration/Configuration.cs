using System;
using Microsoft.Extensions.Configuration;
using Zidium.Common;

namespace Zidium.Core.Single.Tests
{
    internal class Configuration : BaseConfiguration<Options>,
        IDebugConfiguration, IDatabaseConfiguration, IDispatcherConfiguration, ILogicConfiguration, IAccessConfiguration
    {
        public Configuration(IConfiguration configuration) : base(configuration)
        {
        }

        public bool DebugMode => Get().DebugMode;

        public string ProviderName => Get().Database.ProviderName;

        public string ConnectionString => Get().Database.ConnectionString;

        public bool UseLocalDispatcher => Get().UseLocalDispatcher;

        public Uri DispatcherUrl => new Uri(Get().DispatcherUrl);

        public string WebSite => "zidium.net";

        public string SecretKey => Get().SecretKey;

        public string MasterPassword => "Master";

        public int? EventsMaxDays => 6;

        public int? LogMaxDays => 7;

        public int? MetricsMaxDays => 4;

        public int? UnitTestsMaxDays => 5;
    }
}