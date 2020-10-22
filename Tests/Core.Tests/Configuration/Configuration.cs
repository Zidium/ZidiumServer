using System;
using Zidium.Common;
using Zidium.TestTools;

namespace Zidium.Core.Tests
{
    internal class Configuration : JsonConfiguration<Options>,
        IDebugConfiguration, IDatabaseConfiguration, IDispatcherConfiguration, ITestsConfiguration,
        ICoreTestsConfiguration
    {
        public bool DebugMode => Get().DebugMode;

        public string ProviderName => Get().Database.ProviderName;

        public string ConnectionString => Get().Database.ConnectionString;

        public bool UseLocalDispatcher => Get().UseLocalDispatcher;

        public Uri DispatcherUrl => new Uri(Get().DispatcherUrl);

        public string ApiUrl => Get().ApiUrl;

        public string VirusTotalKey => Get().VirusTotalKey;
    }
}