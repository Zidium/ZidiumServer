using System;

namespace Zidium.Storage
{
    public class SendMessageCommandForAdd
    {
        public Guid Id;

        /// <summary>
        /// Канал отправки (мессенджер)
        /// </summary>
        public SubscriptionChannel Channel;

        /// <summary>
        /// Получатель
        /// </summary>
        public string To;

        /// <summary>
        /// Текст
        /// </summary>
        public string Body;

        /// <summary>
        /// Статус
        /// </summary>
        public MessageStatus Status;

        /// <summary>
        /// Сообщение об ошибке отправки, если она есть
        /// </summary>
        public string ErrorMessage;

        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime CreateDate;

        /// <summary>
        /// Дата отправки
        /// </summary>
        public DateTime? SendDate;

        /// <summary>
        /// Ссылка на объект, вызвавший отправку сообщения
        /// </summary>
        public Guid? ReferenceId;
    }
}
