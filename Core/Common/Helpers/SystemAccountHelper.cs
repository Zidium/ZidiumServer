using System;
using ApiAdapter;
using Zidium.Api;
using Zidium.Api.XmlConfig;
using Zidium.Core.Api;
using Zidium.Core.ConfigDb;

namespace Zidium.Core.Common.Helpers
{
    public static class SystemAccountHelper
    {
        public static Zidium.Api.AccessToken GetSystemToken()
        {
            if (_systemToken == null)
            {
                var account = ConfigDbServicesHelper.GetAccountService().GetSystemAccount();
                _systemToken = new Zidium.Api.AccessToken()
                {
                    SecretKey = account.SecretKey
                };
            }

            return _systemToken;
        }

        private static Zidium.Api.AccessToken _systemToken;

        public static Api.AccessToken GetLocalToken(Guid? accountId)
        {
            return new Api.AccessToken()
            {
                AccountId = accountId,
                SecretKey = GetSystemToken().SecretKey,
                ProgramName = "Local",
                IsLocalRequest = true
            };
        }

        public static IClient GetApiClient()
        {
            var config = ConfigHelper.LoadFromXmlOrGetDefault();

            // перезатрем настройки доступа
            var token = GetSystemToken();
            config.Access.AccountName = SystemAccountName;
            config.Access.SecretKey = token.SecretKey;

            return new Client(config);
        }

        public static IClient GetApiClient(IApiService service)
        {
            var config = ConfigHelper.LoadFromXmlOrGetDefault();

            // перезатрем настройки доступа
            var token = GetSystemToken();
            config.Access.AccountName = SystemAccountName;
            config.Access.SecretKey = token.SecretKey;

            return new Client(service, config);
        }

        public static Guid GetSystemAccountId()
        {
            if (_systemAccountId == null)
                _systemAccountId = ConfigDbServicesHelper.GetAccountService().GetSystemAccount().Id;
            return _systemAccountId.Value;
        }

        private static Guid? _systemAccountId;

        /// <summary>
        // Получение специального клиента Api для внутреннего использования
        // Он будет работать через локальный класс сервиса Api, а не обращаться к Web-сервису Api
        // Нет смысла тратить ресурсы web-сервиса Api на мониторинг самого Зидиума
        // Настройки доступа к системному аккаунту берутся не из конфига, а из базы
        /// </summary>
        public static IClient GetInternalSystemClient()
        {
            var dispatcher = DispatcherHelper.GetDispatcherService();
            var dtoService = new ApiToDispatcherAdapter(dispatcher, NetworkHelper.GetLocalIp(), SystemAccountName);
            var apiService = new ApiService(dtoService);
            var config = ConfigHelper.LoadFromXmlOrGetDefault();
            config.Access.AccountName = SystemAccountName;
            config.Access.SecretKey = GetSystemToken().SecretKey;
            var client = new Client(apiService, config);
            return client;
        }

        public static string SystemAccountName = "System";
    }
}
