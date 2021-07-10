using System;

namespace Zidium.Storage
{
    public class SendEmailCommandForAdd
    {
        public Guid Id;

        /// <summary>
        /// От кого
        /// </summary>
        public string From;

        /// <summary>
        /// Кому
        /// </summary>
        public string To;

        /// <summary>
        /// Тема
        /// </summary>
        public string Subject;

        /// <summary>
        /// Текст
        /// </summary>
        public string Body;

        /// <summary>
        /// Html-формат?
        /// </summary>
        public bool IsHtml;

        /// <summary>
        /// Статус
        /// </summary>
        public EmailStatus Status;

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
    }
}
