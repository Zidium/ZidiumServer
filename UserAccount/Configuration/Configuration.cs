using System;
using Microsoft.Extensions.Configuration;
using Zidium.Common;
using Zidium.Core;

namespace Zidium.UserAccount
{
    internal class Configuration : BaseConfiguration<Options>,
        IDebugConfiguration, IDatabaseConfiguration, IDispatcherConfiguration, IAccessConfiguration
    {
        public Configuration(IConfiguration configuration) : base(configuration)
        {
        }

        public bool DebugMode => Get().DebugMode;

        public string ProviderName => Get().Database.ProviderName;

        public string ConnectionString => Get().Database.ConnectionString;

        public bool UseLocalDispatcher => Get().UseLocalDispatcher;

        public Uri DispatcherUrl => new Uri(Get().DispatcherUrl);

        public string SecretKey => Get().SecretKey;
    }
}