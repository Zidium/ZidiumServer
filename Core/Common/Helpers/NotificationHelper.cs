using System;
using System.Linq;
using Zidium.Core.AccountsDb;

namespace Zidium.Core.Common.Helpers
{
    public static class NotificationHelper
    {
        /// <summary>
        /// Отправка уведомления админам аккаунта
        /// </summary>
        public static void NotifyAccountAdmins(Guid accountId, string subject, string body, DatabasesContext dbContext)
        {
            var accountContext = dbContext.GetAccountDbContext(accountId);
            var userRepository = accountContext.GetUserRepository();

            var emailRepository = accountContext.GetSendEmailCommandRepository();

            var admins = userRepository.QueryAll().Where(x => x.Roles.Any(y => y.RoleId == RoleId.AccountAdministrators)).ToList();

            foreach (var admin in admins)
            {
                var letter = HtmlToLetter(body, admin);
                var emails = admin.UserContacts.Where(t => t.Type == UserContactType.Email).Select(t => t.Value).Distinct().ToList();
                foreach (var email in emails)
                {
                    emailRepository.Add(email, subject, letter, accountId);
                }
            }
        }

        /// <summary>
        /// Превращает html в итоговое письмо
        /// Добавляет стандартные для всех писем заголовок и подпись
        /// </summary>
        public static string HtmlToLetter(string text, User user)
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
