using System;

namespace Zidium.Storage
{
    // Событие
    public class EventForRead
    {
        public EventForRead(
            Guid id, 
            Guid ownerId, 
            Guid eventTypeId, 
            string message, 
            EventImportance importance, 
            EventImportance previousImportance, 
            int count, 
            long joinKeyHash, 
            DateTime createDate, 
            DateTime startDate, 
            DateTime endDate, 
            DateTime actualDate, 
            bool isSpace, 
            DateTime lastUpdateDate, 
            DateTime? lastNotificationDate, 
            bool isUserHandled, 
            string version, 
            long? versionLong, 
            EventCategory category, 
            Guid? lastStatusEventId, 
            Guid? firstReasonEventId)
        {
            Id = id;
            OwnerId = ownerId;
            EventTypeId = eventTypeId;
            Message = message;
            Importance = importance;
            PreviousImportance = previousImportance;
            Count = count;
            JoinKeyHash = joinKeyHash;
            CreateDate = createDate;
            StartDate = startDate;
            EndDate = endDate;
            ActualDate = actualDate;
            IsSpace = isSpace;
            LastUpdateDate = lastUpdateDate;
            LastNotificationDate = lastNotificationDate;
            IsUserHandled = isUserHandled;
            Version = version;
            VersionLong = versionLong;
            Category = category;
            LastStatusEventId = lastStatusEventId;
            FirstReasonEventId = firstReasonEventId;
        }

        /// <summary>
        /// Идентификатор события
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// ИД владельца. Например, для события проверки - это ИД проверки.
        /// </summary>
        public Guid OwnerId { get; }

        /// <summary>
        /// Ссылка на тип события
        /// </summary>
        public Guid EventTypeId { get; }

        /// <summary>
        /// Сообщение
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Важность события
        /// </summary>
        public EventImportance Importance { get; }

        /// <summary>
        /// Предыдущая важность. Используется для событий статусов.
        /// </summary>
        public EventImportance PreviousImportance { get; }

        /// <summary>
        /// Счетчик - сколько раз случалось событие
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Хэш ключа по которому склеиваются события
        /// </summary>
        public long JoinKeyHash { get; }

        /// <summary>
        /// Дата создания события в системе мониторинга (время системы)
        /// </summary>
        public DateTime CreateDate { get; }

        /// <summary>
        /// Дата начала события (по местному времени аккаунта)
        /// </summary>
        public DateTime StartDate { get; }

        /// <summary>
        /// Дата завершения события (по местному времени аккаунта)
        /// </summary>
        public DateTime EndDate { get; }

        /// <summary>
        /// Время ДО которого событие можно считать актуальным
        /// </summary>
        public DateTime ActualDate { get; }

        /// <summary>
        /// Признак "событие - пробел" (пробел между колбасками)
        /// </summary>
        public bool IsSpace { get; }

        /// <summary>
        /// Время последнего изменения события.
        /// </summary>
        public DateTime LastUpdateDate { get; }

        /// <summary>
        /// Время последней обработки события для создания уведомлений
        /// </summary>
        public DateTime? LastNotificationDate { get; }

        /// <summary>
        /// Событие обработано человеком
        /// </summary>
        public bool IsUserHandled { get; }

        /// <summary>
        /// Версия компонента на момент появления события
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Версия в виде числа для сравнений
        /// </summary>
        public long? VersionLong { get; }

        /// <summary>
        /// Категория события
        /// </summary>
        public EventCategory Category { get; }

        /// <summary>
        /// Ссылка на последний статус, для которого данное событие является причиной
        /// </summary>
        public Guid? LastStatusEventId { get; }

        /// <summary>
        /// Ссылка на первую причину данного статуса
        /// </summary>
        public Guid? FirstReasonEventId { get; }

        public EventForUpdate GetForUpdate()
        {
            return new EventForUpdate(Id);
        }

    }
}
