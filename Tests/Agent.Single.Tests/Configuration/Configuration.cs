using System;
using Microsoft.Extensions.Configuration;
using Zidium.Common;
using Zidium.Core;

namespace Zidium.Agent.Single.Tests
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

        public string MasterPassword => null;

        public int? EventsMaxDays => 30;

        public int? LogMaxDays => 30;

        public int? MetricsMaxDays => 30;

        public int? UnitTestsMaxDays => 30;
    }
}