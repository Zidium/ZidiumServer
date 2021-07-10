using System;
using System.Net;
using System.Text;
using System.Web;
using Zidium.Storage;

namespace Zidium.Core.Common
{
    /// <summary>
    /// Хелпер для генерации email собщений
    /// </summary>
    public static class EmailMessageHelper
    {
        public static string GetEncodedHtml(string text)
        {
            return WebUtility.HtmlEncode(text);
        }

        public static string GetEncodedUrl(string text)
        {
            return HttpUtility.HtmlEncode(text);
        }

        public static SendEmailCommandForAdd CreateHtmlMessage(string to, string subject)
        {
            return new SendEmailCommandForAdd()
            {
                Id = Guid.NewGuid(),
                To = to,
                From = null,
                IsHtml = true,
                Subject = subject
            };
        }

        public static SendEmailCommandForAdd NewUserLetter(string email, string url)
        {
            var body = new StringBuilder();
            body.AppendLine("<h2>Вам создана учётная запись в системе Zidium!</h2>");
            body.AppendFormat("<p>Ваш логин: {0}</p>", GetEncodedHtml(email));
            body.AppendFormat("<p><a href='{0}'>Для установки пароля перейдите по этой ссылке</a></p>", url);
            var command = CreateHtmlMessage(email, "Создана учётная запись");
            command.Body = HtmlToLetter(body.ToString());
            return command;
        }

        public static SendEmailCommandForAdd ResetPasswordLetter(string email, string url)
        {
            var body = new StringBuilder();
            body.AppendLine("<h2>Здравствуйте!</h2>");
            body.AppendLine("<p>Это письмо для восстановления вашего пароля в системе Zidium</p>");
            body.AppendLine("<p>Если вы не запрашивали восстановление пароля, удалите это письмо</p>");
            body.AppendFormat("<p>Ваш логин: {0}</p>", GetEncodedHtml(email));
            body.AppendFormat("<p><a href='{0}'>Для восстановления пароля перейдите по этой ссылке</a></p>", url);
            var command = CreateHtmlMessage(email, "Восстановление пароля");
            command.Body = HtmlToLetter(body.ToString());
            return command;
        }

        /// <summary>
        /// Превращает html в итоговое письмо
        /// Добавляет стандартные для всех писем заголовок и подпись
        /// </summary>
        /// <returns></returns>
        public static string HtmlToLetter(string text)
        {
            var html = new HtmlRender();
            html.WriteRaw(text);
            html.NewLine();
            html.NewLine();
            html.Write("Ваш Zidium");
            html.NewLine();
            return html.GetHtml();
        }
    }
}
