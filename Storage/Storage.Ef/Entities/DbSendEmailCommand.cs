using System;

namespace Zidium.Storage.Ef
{
    /// <summary>
    /// Команда на отправку EMail
    /// </summary>
    public class DbSendEmailCommand
    {
        public Guid Id { get; set; }

        /// <summary>
        /// От кого
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Кому
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Тема
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Текст
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Html-формат?
        /// </summary>
        public bool IsHtml { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        public EmailStatus Status { get; set; }

        /// <summary>
        /// Сообщение об ошибке отправки, если она есть
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Дата отправки
        /// </summary>
        public DateTime? SendDate { get; set; }

        /// <summary>
        /// Ссылка на объект, породивший отправку письма
        /// Например, письмо со ссылкой активации аккаунта содержит ID заявки на регистрацию
        /// </summary>
        public Guid? ReferenceId { get; set; }

    }
}
