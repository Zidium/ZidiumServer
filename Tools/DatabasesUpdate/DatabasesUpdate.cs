using System;
using System.Linq;
using NLog;
using Zidium.Common;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.ConfigDb;
using Zidium.Storage;
using Zidium.Storage.Ef;

namespace Zidium.DatabasesUpdate
{
    /// <summary>
    /// Класс обновляет базу до последней версии
    /// </summary>
    public static class DatabasesUpdate
    {
        private static ILogger _logger = LogManager.GetCurrentClassLogger();

        public static bool UpdateAll(bool isTestEnviroment)
        {
            try
            {
                Initialization.SetServices();
                StorageFactory.DisableMigrations();

                DependencyInjection.SetServicePersistent<IStorageFactory>(new StorageFactory());
                DependencyInjection.SetServicePersistent<IAccountStorageFactory>(new LocalAccountStorageFactory());

                var accountDbStorageFactory = DependencyInjection.GetServicePersistent<IStorageFactory>();

                var connectionString = DependencyInjection.GetServicePersistent<IDatabaseConfiguration>().ConnectionString;
                var accountDbStorage = accountDbStorageFactory.GetStorage(connectionString);
                var count = accountDbStorage.Migrate();

                // обновляем справочники
                var registrator = new AccountDbDataRegistator(accountDbStorage);
                registrator.RegisterAll();

                // Проверим, создан ли админ
                var userService = new UserService(accountDbStorage);
                var adminCreated = userService.GetAccountAdmins().Any();

                if (count == 0)
                    _logger.Info("База не изменилась");
                else
                    _logger.Info("База обновлена, установлено миграций: " + count);

                if (adminCreated)
                    _logger.Info("Администратор аккаунта уже создан");
                else
                {
                    var configDbServicesFactory = DependencyInjection.GetServicePersistent<IConfigDbServicesFactory>();
                    var accountInfo = configDbServicesFactory.GetAccountService().GetSystemAccount();

                    // создаем root компонент
                    var componentService = new ComponentService(accountDbStorage);
                    componentService.CreateRoot(accountInfo.Id, accountInfo.RootId);

                    // создаем админа
                    string adminEMail;
                    if (!isTestEnviroment)
                    {
                        Console.Write("Укажите EMail администратора: ");
                        adminEMail = Console.ReadLine();
                    }
                    else
                    {
                        adminEMail = "admin@none";
                    }

                    string adminPassword;
                    if (!isTestEnviroment)
                    {
                        Console.Write("Укажите пароль администратора: ");
                        adminPassword = Console.ReadLine();
                    }
                    else
                    {
                        adminPassword = Guid.NewGuid().ToString();
                    }

                    var adminUserId = userService.CreateAccountAdmin(
                        accountInfo.Id,
                        adminEMail,
                        null, null, null, null, null);

                    // Установим пароль админа
                    var passwordToken = userService.StartResetPassword(adminUserId, false);
                    userService.EndResetPassword(accountInfo.Id, passwordToken, adminPassword);

                    _logger.Info($"Создан пользователь {adminEMail} с паролем {adminPassword}");

                    // Установим бесконечные лимиты аккаунта
                    var limits = SystemTariffLimit.BaseUnlimited;

                    var tariff = new TariffLimitForAdd()
                    {
                        Id = Guid.NewGuid(),
                        Type = TariffLimitType.Soft,
                        Source = TariffLimitSource.Base,
                        Name = "Soft",
                        EventsRequestsPerDay = limits.EventRequestsPerDay,
                        EventsMaxDays = limits.EventsMaxDays,
                        LogMaxDays = limits.LogMaxDays,
                        LogSizePerDay = limits.LogSizePerDay,
                        ComponentsMax = limits.ComponentsMax,
                        ComponentTypesMax = limits.ComponentTypesMax,
                        UnitTestTypesMax = limits.UnitTestTypesMax,
                        HttpUnitTestsMaxNoBanner = limits.HttpChecksMaxNoBanner,
                        UnitTestsMax = limits.UnitTestsMax,
                        UnitTestsRequestsPerDay = limits.UnitTestsRequestsPerDay,
                        UnitTestsMaxDays = limits.UnitTestsMaxDays,
                        MetricsMax = limits.MetricsMax,
                        MetricsRequestsPerDay = limits.MetricRequestsPerDay,
                        MetricsMaxDays = limits.MetricsMaxDays,
                        StorageSizeMax = limits.StorageSizeMax,
                        SmsPerDay = limits.SmsPerDay
                    };

                    accountDbStorage.TariffLimits.Add(tariff);

                    tariff = new TariffLimitForAdd()
                    {
                        Id = Guid.NewGuid(),
                        Type = TariffLimitType.Hard,
                        Source = TariffLimitSource.Base,
                        Name = "Hard",
                        EventsRequestsPerDay = limits.EventRequestsPerDay,
                        EventsMaxDays = limits.EventsMaxDays,
                        LogMaxDays = limits.LogMaxDays,
                        LogSizePerDay = limits.LogSizePerDay,
                        ComponentsMax = limits.ComponentsMax,
                        ComponentTypesMax = limits.ComponentTypesMax,
                        UnitTestTypesMax = limits.UnitTestTypesMax,
                        HttpUnitTestsMaxNoBanner = limits.HttpChecksMaxNoBanner,
                        UnitTestsMax = limits.UnitTestsMax,
                        UnitTestsRequestsPerDay = limits.UnitTestsRequestsPerDay,
                        UnitTestsMaxDays = limits.UnitTestsMaxDays,
                        MetricsMax = limits.MetricsMax,
                        MetricsRequestsPerDay = limits.MetricRequestsPerDay,
                        MetricsMaxDays = limits.MetricsMaxDays,
                        StorageSizeMax = limits.StorageSizeMax,
                        SmsPerDay = limits.SmsPerDay
                    };

                    accountDbStorage.TariffLimits.Add(tariff);

                }

                _logger.Info("База успешно обновлена");

                return true;
            }
            catch (Exception exception)
            {
                _logger.Fatal(exception);
                return false;
            }
        }
    }
}
