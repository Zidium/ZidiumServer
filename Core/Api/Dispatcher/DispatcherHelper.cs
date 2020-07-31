using System;
using System.Configuration;
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
            var value = ConfigurationManager.AppSettings["UseLocalDispatcher"];
            return value != null && value.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        public static Uri GetDispatcherUri()
        {
            string uri = ConfigurationManager.AppSettings["DispatcherUrl"];
            if (string.IsNullOrEmpty(uri))
            {
                throw new ArgumentException("Не задан параметр DispatcherUrl");
            }
            return new Uri(uri);
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
    }
}
