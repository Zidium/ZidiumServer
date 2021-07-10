using System;
using Zidium.Core.Api.Dispatcher;

namespace Zidium.Core.Api
{
    public class DispatcherHelper
    {
        public static bool UseLocalDispatcher()
        {
            return CoreConfiguration.UseLocalDispatcher;
        }

        public static Uri GetDispatcherUri()
        {
            return CoreConfiguration.DispatcherUrl;
        }

        protected static IDispatcherService GetDispatcherService(bool useLocalDispatcher)
        {
            if (useLocalDispatcher)
            {
                // это настоящий диспетчер (локальный)
                return DispatcherService.Wrapper;
            }

            // это прокси веб-службы диспетчера
            Uri uri = GetDispatcherUri();
            return new DispatcherServiceProxy(uri);
        }

        public static IDispatcherService GetDispatcherService()
        {
            bool useLocalDispatcher = UseLocalDispatcher();
            return GetDispatcherService(useLocalDispatcher);
        }

        public static DispatcherClient GetDispatcherClient()
        {
            return _dispatcherClient;
        }

        private static readonly DispatcherClient _dispatcherClient = new DispatcherClient("Core");

        private static IDispatcherConfiguration _coreConfiguration;

        private static IDispatcherConfiguration CoreConfiguration
        {
            get
            {
                if (_coreConfiguration == null)
                {
                    _coreConfiguration = DependencyInjection.GetServicePersistent<IDispatcherConfiguration>();
                }

                return _coreConfiguration;
            }
        }

    }
}
