using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    public class EventForAdd
    {
        /// <summary>
        /// Идентификатор события
        /// </summary>
        public Guid Id;

        /// <summary>
        /// ИД владельца. Например, для события проверки - это ИД проверки.
        /// </summary>
        public Guid OwnerId;

        /// <summary>
        /// Ссылка на тип события
        /// </summary>
        public Guid EventTypeId;

        /// <summary>
        /// Сообщение
        /// </summary>
        public string Message;

        /// <summary>
        /// Важность события
        /// </summary>
        public EventImportance Importance;

        /// <summary>
        /// Предыдущая важность. Используется для событий статусов.
        /// </summary>
        public EventImportance PreviousImportance;

        /// <summary>
        /// Счетчик - сколько раз случалось событие
        /// </summary>
        public int Count;

        /// <summary>
        /// Хэш ключа по которому склеиваются события
        /// </summary>
        public long JoinKeyHash;

        /// <summary>
        /// Дата создания события в системе мониторинга (время системы)
        /// </summary>
        public DateTime CreateDate;

        /// <summary>
        /// Дата начала события (по местному времени аккаунта)
        /// </summary>
        public DateTime StartDate;

        /// <summary>
        /// Дата завершения события (по местному времени аккаунта)
        /// </summary>
        public DateTime EndDate;

        /// <summary>
        /// Время ДО которого событие можно считать актуальным
        /// </summary>
        public DateTime ActualDate;

        /// <summary>
        /// Признак "событие - пробел" (пробел между колбасками)
        /// </summary>
        public bool IsSpace;

        /// <summary>
        /// Время последнего изменения события.
        /// </summary>
        public DateTime LastUpdateDate;

        /// <summary>
        /// Время последней обработки события для создания уведомлений
        /// </summary>
        public DateTime? LastNotificationDate;

        /// <summary>
        /// Событие обработано человеком
        /// </summary>
        public bool IsUserHandled;

        /// <summary>
        /// Версия компонента на момент появления события
        /// </summary>
        public string Version;

        /// <summary>
        /// Версия в виде числа для сравнений
        /// </summary>
        public long? VersionLong;

        /// <summary>
        /// Категория события
        /// </summary>
        public EventCategory Category;

        /// <summary>
        /// Ссылка на последний статус, для которого данное событие является причиной
        /// </summary>
        public Guid? LastStatusEventId;

        /// <summary>
        /// Ссылка на первую причину данного статуса
        /// </summary>
        public Guid? FirstReasonEventId;

        public EventPropertyForAdd[] Properties;

    }
}
