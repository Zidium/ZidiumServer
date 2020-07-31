using System;

namespace Zidium.Storage.Ef
{
    /// <summary>
    /// Команда на отправку сообщения через мессенджер
    /// </summary>
    public class DbSendMessageCommand
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Канал отправки (мессенджер)
        /// </summary>
        public SubscriptionChannel Channel { get; set; }

        /// <summary>
        /// Получатель
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// Текст
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        public MessageStatus Status { get; set; }

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
        /// Ссылка на объект, вызвавший отправку сообщения
        /// </summary>
        public Guid? ReferenceId { get; set; }

    }
}