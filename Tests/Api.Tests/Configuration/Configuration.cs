using System;
using Microsoft.Extensions.Configuration;
using Zidium.Common;
using Zidium.Core;

namespace Zidium.Api.Tests
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

        public int? EventsMaxDays => null;

        public int? LogMaxDays => null;

        public int? MetricsMaxDays => null;

        public int? UnitTestsMaxDays => null;
    }
}