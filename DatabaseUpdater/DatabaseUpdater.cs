using System;
using NLog;
using Zidium.Common;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Storage;
using Zidium.Storage.Ef;

namespace Zidium.DatabaseUpdater
{
    /// <summary>
    /// Класс обновляет базу до последней версии
    /// </summary>
    public static class DatabaseUpdater
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public static bool UpdateAll(bool isTestEnviroment)
        {
            try
            {
                DependencyInjection.SetServicePersistent<IStorageFactory>(new StorageFactory());
                DependencyInjection.SetServicePersistent<IDefaultStorageFactory>(new DefaultStorageFactory());

                var storageFactory = DependencyInjection.GetServicePersistent<IStorageFactory>();

                var connectionString = DependencyInjection.GetServicePersistent<IDatabaseConfiguration>().ConnectionString;
                var storage = storageFactory.GetStorage(connectionString);
                var count = storage.Migrate(_logger);

                if (count == 0)
                    _logger.Info("База не изменилась");
                else
                    _logger.Info("База обновлена, установлено миграций: " + count);

                // обновляем справочники
                var registrator = new AccountDbDataRegistator(storage);
                registrator.RegisterAll();

                // Проверим, создан ли админ
                var userService = new UserService(storage);
                var adminCreated = userService.GetAccountAdmins().Length > 0;

                if (adminCreated)
                    _logger.Info("Администратор аккаунта уже создан");
                else
                {
                    // создаем root компонент
                    var componentService = new ComponentService(storage);
                    componentService.CreateRoot(SystemComponentType.Root.Id);

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
                        adminEMail,
                        null, null, null, null, null);

                    // Установим пароль админа
                    var passwordToken = userService.StartResetPassword(adminUserId, false);
                    userService.EndResetPassword(passwordToken, adminPassword);

                    _logger.Info($"Создан пользователь {adminEMail} с паролем {adminPassword}");
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
