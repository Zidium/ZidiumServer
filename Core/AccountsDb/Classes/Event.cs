using System;
using System.Collections.Generic;
using Zidium.Core.Api;
using Zidium.Core.Common.Helpers;

namespace Zidium.Core.AccountsDb
{
    // Событие
    public class Event
    {
        public Event()
        {
            Properties = new HashSet<EventProperty>();
            Notifications = new HashSet<Notification>();
            StatusEvents = new HashSet<Event>();
            ReasonEvents = new HashSet<Event>();
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
        /// Тпи события
        /// </summary>
        public virtual EventType EventType { get; set; }

        /// <summary>
        /// Сообщение
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Юнит-тесты могут НЕ передавать сообщение в результате, поэтому для таких результатов сгенерируем message сами
        /// </summary>
        /// <returns></returns>
        public string GetUnitTestMessage()
        {
            if (string.IsNullOrEmpty(Message))
            {
                if (Importance == EventImportance.Alarm)
                {
                    return "alarm";
                }
                if (Importance == EventImportance.Warning)
                {
                    return "warning";
                }
                if (Importance == EventImportance.Success)
                {
                    return "success";
                }
                if (Importance == EventImportance.Unknown)
                {
                    return "unknown";
                }
            }
            return Message;
        }

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

        public DateTime GetEndDate(DateTime now)
        {
            return StartDate + EventHelper.GetDuration(StartDate, ActualDate, DateTime.Now);
        }

        /// <summary>
        /// Время ДО которого событие можно считать актуальным
        /// </summary>
        public DateTime ActualDate { get; set; }

        /// <summary>
        /// Признак "событие - пробел" (пробел между колбасками)
        /// </summary>
        public bool IsSpace { get; set; }

        /// <summary>
        /// Статус стал более важным (опасным). Используется для событий статусов.
        /// </summary>
        public bool ImportanceHasGrown
        {
            get
            {
                return Importance > PreviousImportance;
            }
        }

        /// <summary>
        /// Дополнительные параметры события
        /// </summary>
        public virtual ICollection<EventProperty> Properties { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }

        public virtual ICollection<Event> StatusEvents { get; set; }

        public virtual ICollection<Event> ReasonEvents { get; set; }

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
        /// Длительность события
        /// </summary>
        public TimeSpan Duration
        {
            get { return GetDuration(DateTime.Now); }
        }

        public TimeSpan GetDuration(DateTime now)
        {
            return EventHelper.GetDuration(StartDate, ActualDate, now);
        }

        /// <summary>
        /// Ссылка на последний статус, для которого данное событие является причиной
        /// </summary>
        public Guid? LastStatusEventId { get; set; }

        /// <summary>
        /// Последний статус, для которого данное событие является причиной
        /// </summary>
        public virtual Event LastStatusEvent { get; set; }

        /// <summary>
        /// Ссылка на первую причину данного статуса
        /// </summary>
        public Guid? FirstReasonEventId { get; set; }

        public TimeSpan GetActualInterval()
        {
            return ActualDate - EndDate;
        }
    }
}
