using System;
using System.Configuration;
using System.Linq;
using NLog;
using Zidium.Core.AccountsDb;
using Zidium.Core.ConfigDb;
using Zidium.Core.DispatcherLayer;

namespace DatabasesUpdate
{
    /// <summary>
    /// Класс обновляет базу до последней версии
    /// </summary>
    public static class DatabasesUpdate
    {
        private static ILogger _logger = LogManager.GetCurrentClassLogger();

        public static bool UpdateAll(string sectionName, bool isTestEnviroment)
        {
            try
            {
                Initialization.SetServices();
                Provider.SetSectionName(sectionName);
                var connectionString = ConfigurationManager.ConnectionStrings[sectionName].ConnectionString;
                DatabaseService.SetConnectionString(connectionString);
                var provider = Provider.Current();
                provider.Configuration();

                var count = provider.InitializeDb(connectionString);

                bool adminCreated;
                using (var accountDbContext = AccountDbContext.CreateFromConnectionString(connectionString))
                {
                    // обновляем справочники
                    var registrator = new AccountDbDataRegistator(accountDbContext);
                    registrator.RegisterAll();

                    // Проверим, создан ли админ
                    adminCreated = accountDbContext.GetUserRepository().QueryAll().FirstOrDefault(x => x.Roles.Any(y => y.RoleId == RoleId.AccountAdministrators)) != null;
                }

                if (count == 0)
                    _logger.Info("База не изменилась");
                else
                    _logger.Info("База обновлена, установлено миграций: " + count);

                if (adminCreated)
                    _logger.Info("Администратор аккаунта уже создан");
                else
                {
                    var accountInfo = ConfigDbServicesHelper.GetAccountService().GetSystemAccount();

                    using (var context = DispatcherContext.Create())
                    {
                        // создаем root компонент
                        var componentService = context.ComponentService;
                        componentService.CreateRoot(accountInfo.Id, accountInfo.RootId);

                        // создаем админа
                        Console.Write("Укажите EMail администратора: ");
                        var adminEMail = Console.ReadLine();

                        Console.Write("Укажите пароль администратора: ");
                        var adminPassword = Console.ReadLine();

                        var userService = context.UserService;

                        var adminUser = userService.CreateAccountAdmin(
                            accountInfo.Id,
                            adminEMail,
                            null, null, null, null, null);

                        // Установим пароль админа
                        var passwordToken = userService.StartResetPassword(adminUser.Id, false);
                        userService.EndResetPassword(accountInfo.Id, passwordToken, adminPassword);
                        context.SaveChanges();

                        _logger.Info($"Создан пользователь {adminEMail} с паролем {adminPassword}");
                    }

                    // Установим бесконечные лимиты аккаунта
                    var limits = SystemTariffLimits.BaseUnlimited;

                    using (var accountDbContext = AccountDbContext.CreateFromConnectionString(connectionString))
                    {
                        var accountTariffRepository = accountDbContext.GetAccountTariffRepository();
                        var tariffLimitRepository = accountDbContext.GetTariffLimitRepository();

                        var tariff = new TariffLimit()
                        {
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
                            SmsPerDay = limits.SmsPerDay,

                        };

                        tariffLimitRepository.Add(tariff);

                        var accountTariff = new AccountTariff()
                        {
                            TariffLimit = tariff
                        };
                        accountTariffRepository.Add(accountTariff);

                        tariff = new TariffLimit()
                        {
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
                            SmsPerDay = limits.SmsPerDay,

                        };

                        tariffLimitRepository.Add(tariff);

                        accountTariff = new AccountTariff()
                        {
                            TariffLimit = tariff
                        };
                        accountTariffRepository.Add(accountTariff);

                        accountDbContext.SaveChanges();
                    }

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
