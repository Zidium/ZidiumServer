using System;

namespace Zidium.Storage
{
    public class SendSmsCommandForAdd
    {
        public Guid Id;

        /// <summary>
        /// Телефон
        /// </summary>
        public string Phone;

        /// <summary>
        /// Текст
        /// </summary>
        public string Body;

        /// <summary>
        /// Статус
        /// </summary>
        public SmsStatus Status;

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
        /// Ссылка на объект, породивший отправку письма
        /// Например, письмо со ссылкой активации аккаунта содержит ID заявки на регистрацию
        /// </summary>
        public Guid? ReferenceId;

        /// <summary>
        /// Внешний id в шлюзе
        /// </summary>
        public string ExternalId;
    }
}
