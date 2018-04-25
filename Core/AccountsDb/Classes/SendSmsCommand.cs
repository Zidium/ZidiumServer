using System;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Команда на отправку SMS
    /// </summary>
    public class SendSmsCommand
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Телефон
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Текст
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        public SmsStatus Status { get; set; }

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

        /// <summary>
        /// Внешний id в шлюзе
        /// </summary>
        public string ExternalId { get; set; }
    }
}
