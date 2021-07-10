using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Команда на отправку сообщения через мессенджер
    /// </summary>
    public class SendMessageCommandForRead
    {
        public SendMessageCommandForRead(
            Guid id, 
            SubscriptionChannel channel, 
            string to, 
            string body, 
            MessageStatus status, 
            string errorMessage, 
            DateTime createDate, 
            DateTime? sendDate, 
            Guid? referenceId)
        {
            Id = id;
            Channel = channel;
            To = to;
            Body = body;
            Status = status;
            ErrorMessage = errorMessage;
            CreateDate = createDate;
            SendDate = sendDate;
            ReferenceId = referenceId;
        }

        public Guid Id { get; }

        /// <summary>
        /// Канал отправки (мессенджер)
        /// </summary>
        public SubscriptionChannel Channel { get; }

        /// <summary>
        /// Получатель
        /// </summary>
        public string To { get; }

        /// <summary>
        /// Текст
        /// </summary>
        public string Body { get; }

        /// <summary>
        /// Статус
        /// </summary>
        public MessageStatus Status { get; }

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
        /// Ссылка на объект, вызвавший отправку сообщения
        /// </summary>
        public Guid? ReferenceId { get; }

    }
}