using System;
using Zidium.Common;
using Zidium.Core;
using Zidium.TestTools;

namespace ApiTests_1._0
{
    internal class Configuration : JsonConfiguration<Options>,
        IDebugConfiguration, IDatabaseConfiguration, IDispatcherConfiguration, ITestsConfiguration
    {
        public bool DebugMode => Get().DebugMode;

        public string ProviderName => Get().Database.ProviderName;

        public string ConnectionString => Get().Database.ConnectionString;

        public bool UseLocalDispatcher => Get().UseLocalDispatcher;

        public Uri DispatcherUrl => new Uri(Get().DispatcherUrl);

        public string ApiUrl => Get().ApiUrl;
    }
}