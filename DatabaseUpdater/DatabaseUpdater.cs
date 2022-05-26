using System;
using Microsoft.Extensions.Logging;
using Zidium.Common;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.Storage;
using Zidium.Storage.Ef;

namespace Zidium.DatabaseUpdater
{
    /// <summary>
    /// Класс обновляет базу до последней версии
    /// </summary>
    public class DatabaseUpdater
    {
        public bool UpdateAll(bool isTestEnviroment)
        {
            var logger = DependencyInjection.GetLogger<DatabaseUpdater>();
            try
            {
                var storageFactory = new StorageFactory();
                DependencyInjection.SetServicePersistent<IStorageFactory>(storageFactory);
                var timeService = DependencyInjection.GetServicePersistent<ITimeService>();

                var storage = storageFactory.GetStorage();
                var count = storage.Migrate();

                if (count == 0)
                    logger.LogInformation("База не изменилась");
                else
                    logger.LogInformation("База обновлена, установлено миграций: " + count);

                // обновляем справочники
                var registrator = new AccountDbDataRegistator(storage);
                registrator.RegisterAll();

                // Проверим, создан ли админ
                var userService = new UserService(storage, timeService);
                var adminCreated = userService.GetAccountAdmins().Length > 0;

                if (adminCreated)
                    logger.LogInformation("Администратор аккаунта уже создан");
                else
                {
                    // создаем root компонент
                    var componentService = new ComponentService(storage, timeService);
                    componentService.CreateRoot(SystemComponentType.Root.Id);

                    // создаем админа
                    string adminLogin;
                    if (!isTestEnviroment)
                    {
                        Console.Write("Укажите логин администратора: ");
                        adminLogin = Console.ReadLine();
                    }
                    else
                    {
                        adminLogin = "admin";
                    }

                    string adminPassword;
                    if (!isTestEnviroment)
                    {
                        Console.Write("Укажите пароль администратора: ");
                        adminPassword = Console.ReadLine();
                    }
                    else
                    {
                        adminPassword = Ulid.NewUlid().ToString();
                    }

                    var adminUserId = userService.CreateAccountAdmin(
                        adminLogin,
                        null,
                        null);

                    // Установим пароль админа
                    var passwordToken = userService.StartResetPassword(adminUserId, false);
                    userService.EndResetPassword(passwordToken, adminPassword);

                    logger.LogInformation($"Создан пользователь {adminLogin} с паролем {adminPassword}");
                }

                logger.LogInformation("База успешно обновлена");

                return true;
            }
            catch (Exception exception)
            {
                logger.LogCritical(exception, exception.Message);
                return false;
            }
        }
    }
}
