using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Команда на отправку EMail
    /// </summary>
    public class SendEmailCommandForRead
    {
        public SendEmailCommandForRead(
            Guid id, 
            string from, 
            string to, 
            string subject, 
            string body, 
            bool isHtml, 
            EmailStatus status, 
            string errorMessage, 
            DateTime createDate, 
            DateTime? sendDate, 
            Guid? referenceId)
        {
            Id = id;
            From = from;
            To = to;
            Subject = subject;
            Body = body;
            IsHtml = isHtml;
            Status = status;
            ErrorMessage = errorMessage;
            CreateDate = createDate;
            SendDate = sendDate;
            ReferenceId = referenceId;
        }

        public Guid Id { get; }

        /// <summary>
        /// От кого
        /// </summary>
        public string From { get; }

        /// <summary>
        /// Кому
        /// </summary>
        public string To { get; }

        /// <summary>
        /// Тема
        /// </summary>
        public string Subject { get; }

        /// <summary>
        /// Текст
        /// </summary>
        public string Body { get; }

        /// <summary>
        /// Html-формат?
        /// </summary>
        public bool IsHtml { get; }

        /// <summary>
        /// Статус
        /// </summary>
        public EmailStatus Status { get; }

        /// <summary>
        /// Сообщение об ошибке отправки, если она есть
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreateDate { get; }

        /// <summary>
        /// Дата отправки
        /// </summary>
        public DateTime? SendDate { get; }

        /// <summary>
        /// Ссылка на объект, породивший отправку письма
        /// Например, письмо со ссылкой активации аккаунта содержит ID заявки на регистрацию
        /// </summary>
        public Guid? ReferenceId { get; }

    }
}
