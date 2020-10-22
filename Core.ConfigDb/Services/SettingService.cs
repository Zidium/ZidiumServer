using System;

namespace Zidium.Core.ConfigDb
{
    public class SettingService : ISettingService
    {
        public string GetAccountWebSite()
        {
            if (_accountWebSite == null)
            {
                _accountWebSite = DependencyInjection.GetServicePersistent<IConfigDbConfiguration>().AccountWebSite;
                if (_accountWebSite == null)
                    throw new Exception("Не заполнена настройка AccountWebSite в файле конфигурации");
            }

            return _accountWebSite;
        }

        private static string _accountWebSite;
    }
}
