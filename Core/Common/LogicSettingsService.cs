using System;

namespace Zidium.Core.Common
{
    internal class LogicSettingsService : ILogicSettingsService
    {
        public string WebSite()
        {
            if (_accountWebSite == null)
            {
                _accountWebSite = DependencyInjection.GetServicePersistent<ILogicConfiguration>().WebSite;
                if (_accountWebSite == null)
                    throw new Exception("Не заполнена настройка AccountWebSite в файле конфигурации");
            }

            return _accountWebSite;
        }

        private static string _accountWebSite;

        public string MasterPassword()
        {
            if (_masterPassword == null)
            {
                _masterPassword = DependencyInjection.GetServicePersistent<ILogicConfiguration>().MasterPassword;
            }

            return _masterPassword;
        }

        private static string _masterPassword;

        public int EventsMaxDays()
        {
            if (_eventsMaxDays == 0)
            {
                _eventsMaxDays = DependencyInjection.GetServicePersistent<ILogicConfiguration>().EventsMaxDays ?? 30;
            }

            return _eventsMaxDays;
        }

        private static int _eventsMaxDays;

        public int LogMaxDays()
        {
            if (_logMaxDays == 0)
            {
                _logMaxDays = DependencyInjection.GetServicePersistent<ILogicConfiguration>().LogMaxDays ?? 30;
            }

            return _logMaxDays;
        }

        private static int _logMaxDays;

        public int MetricsMaxDays()
        {
            if (_metricsMaxDays == 0)
            {
                _metricsMaxDays = DependencyInjection.GetServicePersistent<ILogicConfiguration>().MetricsMaxDays ?? 30;
            }

            return _metricsMaxDays;
        }

        private static int _metricsMaxDays;

        public int UnitTestsMaxDays()
        {
            if (_unitTestsMaxDays == 0)
            {
                _unitTestsMaxDays = DependencyInjection.GetServicePersistent<ILogicConfiguration>().UnitTestsMaxDays ?? 30;
            }

            return _unitTestsMaxDays;
        }

        private static int _unitTestsMaxDays;
    }
}
