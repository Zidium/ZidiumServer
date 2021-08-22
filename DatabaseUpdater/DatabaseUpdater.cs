﻿using System;
using Microsoft.Extensions.Logging;
using Zidium.Core.AccountsDb;
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
                var userService = new UserService(storage);
                var adminCreated = userService.GetAccountAdmins().Length > 0;

                if (adminCreated)
                    logger.LogInformation("Администратор аккаунта уже создан");
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

                    logger.LogInformation($"Создан пользователь {adminEMail} с паролем {adminPassword}");
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
