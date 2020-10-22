using System;
using Zidium.Core.Api.Dispatcher;
using Zidium.Core.ConfigDb;

namespace Zidium.Core.Api
{
    /// <summary>
    /// Делает более простым вызовы диспетчера, т.к. прячет внутри себя получение и отправку AccessToken
    /// </summary>
    public class DispatcherHelper
    {
        private static AccessToken _accessToken;

        public static AccessToken AccessToken
        {
            get
            {
                if (_accessToken == null)
                {
                    var configDbServicesFactory = DependencyInjection.GetServicePersistent<IConfigDbServicesFactory>();
                    var account = configDbServicesFactory.GetAccountService().GetSystemAccount();
                    _accessToken = new AccessToken()
                    {
                        AccountId = account.Id,
                        SecretKey = account.SecretKey
                    };
                }
                return _accessToken;
            }
        }

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
