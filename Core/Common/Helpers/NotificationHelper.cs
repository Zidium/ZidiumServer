﻿using System;
using System.Linq;
using Zidium.Core.AccountsDb;
using Zidium.Storage;

namespace Zidium.Core.Common.Helpers
{
    public static class NotificationHelper
    {
        /// <summary>
        /// Отправка уведомления админам аккаунта
        /// </summary>
        public static void NotifyAccountAdmins(Guid accountId, string subject, string body)
        {
            var accountStorageFactory = DependencyInjection.GetServicePersistent<IAccountStorageFactory>();
            var storage = accountStorageFactory.GetStorageByAccountId(accountId);

            var userService = new UserService(storage);
            var admins = userService.GetAccountAdmins();

            foreach (var admin in admins)
            {
                var letter = HtmlToLetter(body, admin);
                var emails = storage.UserContacts.GetByType(admin.Id, UserContactType.Email).Select(t => t.Value).Distinct().ToList();
                foreach (var email in emails)
                {
                    var sendEmailCommand = new SendEmailCommandForAdd()
                    {
                        Id = Guid.NewGuid(),
                        To = email,
                        Subject = subject,
                        Body = letter,
                        ReferenceId = accountId,
                        Status = EmailStatus.InQueue,
                        CreateDate = DateTime.Now,
                        IsHtml = true
                    };
                    storage.SendEmailCommands.Add(sendEmailCommand);
                }
            }
        }

        /// <summary>
        /// Превращает html в итоговое письмо
        /// Добавляет стандартные для всех писем заголовок и подпись
        /// </summary>
        public static string HtmlToLetter(string text, UserForRead user)
        {
            var html = new HtmlRender();

            html.Write("Уважаемый " + (!string.IsNullOrEmpty(user.FirstName) || !string.IsNullOrEmpty(user.MiddleName) ? user.FirstName + " " + user.MiddleName : user.Login) + "!");

            html.NewLine();
            html.NewLine();

            html.WriteRaw(text);

            html.NewLine();
            html.NewLine();
            html.Write("Ваш Zidium");
            html.NewLine();

            return html.GetHtml();
        }
    }
}
