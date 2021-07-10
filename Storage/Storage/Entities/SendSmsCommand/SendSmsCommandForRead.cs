using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Команда на отправку SMS
    /// </summary>
    public class SendSmsCommandForRead
    {
        public SendSmsCommandForRead(
            Guid id, 
            string phone, 
            string body, 
            SmsStatus status, 
            string errorMessage, 
            DateTime createDate, 
            DateTime? sendDate, 
            Guid? referenceId, 
            string externalId)
        {
            Id = id;
            Phone = phone;
            Body = body;
            Status = status;
            ErrorMessage = errorMessage;
            CreateDate = createDate;
            SendDate = sendDate;
            ReferenceId = referenceId;
            ExternalId = externalId;
        }

        public Guid Id { get; }

        /// <summary>
        /// Телефон
        /// </summary>
        public string Phone { get; }

        /// <summary>
        /// Текст
        /// </summary>
        public string Body { get; }

        /// <summary>
        /// Статус
        /// </summary>
        public SmsStatus Status { get; }

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

        /// <summary>
        /// Внешний id в шлюзе
        /// </summary>
        public string ExternalId { get; }

    }
}
