using System;
using System.Collections.Generic;

namespace Zidium.Storage.Ef
{
    // Событие
    public class DbEvent
    {
        public DbEvent()
        {
            Properties = new HashSet<DbEventProperty>();
            Notifications = new HashSet<DbNotification>();
            StatusEvents = new HashSet<DbEvent>();
            ReasonEvents = new HashSet<DbEvent>();
        }

        /// <summary>
        /// Идентификатор события
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ИД владельца. Например, для события проверки - это ИД проверки.
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Ссылка на тип события
        /// </summary>
        public Guid EventTypeId { get; set; }

        /// <summary>
        /// Тип события
        /// </summary>
        public virtual DbEventType EventType { get; set; }

        /// <summary>
        /// Сообщение
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Важность события
        /// </summary>
        public EventImportance Importance { get; set; }

        /// <summary>
        /// Предыдущая важность. Используется для событий статусов.
        /// </summary>
        public EventImportance PreviousImportance { get; set; }

        /// <summary>
        /// Счетчик - сколько раз случалось событие
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Хэш ключа по которому склеиваются события
        /// </summary>
        public long JoinKeyHash { get; set; }

        /// <summary>
        /// Дата создания события в системе мониторинга (время системы)
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Дата начала события (по местному времени аккаунта)
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Дата завершения события (по местному времени аккаунта)
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Время ДО которого событие можно считать актуальным
        /// </summary>
        public DateTime ActualDate { get; set; }

        /// <summary>
        /// Признак "событие - пробел" (пробел между колбасками)
        /// </summary>
        public bool IsSpace { get; set; }

        /// <summary>
        /// Дополнительные параметры события
        /// </summary>
        public virtual ICollection<DbEventProperty> Properties { get; set; }

        public virtual ICollection<DbNotification> Notifications { get; set; }

        public virtual ICollection<DbEvent> StatusEvents { get; set; }

        public virtual ICollection<DbEvent> ReasonEvents { get; set; }

        /// <summary>
        /// Время последнего изменения события.
        /// </summary>
        public DateTime LastUpdateDate { get; set; }

        /// <summary>
        /// Время последней обработки события для создания уведомлений
        /// </summary>
        public DateTime? LastNotificationDate { get; set; }

        /// <summary>
        /// Событие обработано человеком
        /// </summary>
        public bool IsUserHandled { get; set; }

        /// <summary>
        /// Версия компонента на момент появления события
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Версия в виде числа для сравнений
        /// </summary>
        public long? VersionLong { get; set; }

        /// <summary>
        /// Категория события
        /// </summary>
        public EventCategory Category { get; set; }

        /// <summary>
        /// Ссылка на последний статус, для которого данное событие является причиной
        /// </summary>
        public Guid? LastStatusEventId { get; set; }

        /// <summary>
        /// Последний статус, для которого данное событие является причиной
        /// </summary>
        public virtual DbEvent LastStatusEvent { get; set; }

        /// <summary>
        /// Ссылка на первую причину данного статуса
        /// </summary>
        public Guid? FirstReasonEventId { get; set; }

    }
}
