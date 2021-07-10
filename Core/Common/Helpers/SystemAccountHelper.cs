using Zidium.Api;
using Zidium.Api.Dto;
using Zidium.Api.XmlConfig;
using Zidium.Core.Api;

namespace Zidium.Core.Common.Helpers
{
    public static class SystemAccountHelper
    {
        public static AccessTokenDto GetApiToken()
        {
            if (_apiToken == null)
            {
                var accessConfiguration = DependencyInjection.GetServicePersistent<IAccessConfiguration>();
                _apiToken = new AccessTokenDto()
                {
                    SecretKey = accessConfiguration.SecretKey
                };
            }

            return _apiToken;
        }

        private static AccessTokenDto _apiToken;

        public static IClient GetApiClient()
        {
            var config = ConfigHelper.LoadFromXmlOrGetDefault();

            // перезатрем настройки доступа
            var token = GetApiToken();
            config.Access.SecretKey = token.SecretKey;

            return new Client(config);
        }

        public static IClient GetApiClient(IApiService service)
        {
            var config = ConfigHelper.LoadFromXmlOrGetDefault();

            // перезатрем настройки доступа
            var token = GetApiToken();
            config.Access.SecretKey = token.SecretKey;

            return new Client(service, config);
        }

        /// <summary>
        /// Получение специального клиента Api для внутреннего использования
        /// </summary>
        public static IClient GetInternalSystemClient()
        {
            var dispatcher = DispatcherHelper.GetDispatcherService();
            var apiService = new ApiService(dispatcher);
            var config = ConfigHelper.LoadFromXmlOrGetDefault();
            config.Access.SecretKey = GetApiToken().SecretKey;
            var client = new Client(apiService, config);
            return client;
        }
    }
}
