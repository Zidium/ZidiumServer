using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    public class EventForUpdate
    {
        public EventForUpdate(Guid id)
        {
            Id = id;
            Count = new ChangeTracker<int>();
            Message = new ChangeTracker<string>();
            Importance = new ChangeTracker<EventImportance>();
            StartDate = new ChangeTracker<DateTime>();
            EndDate = new ChangeTracker<DateTime>();
            ActualDate = new ChangeTracker<DateTime>();
            LastUpdateDate = new ChangeTracker<DateTime>();
            LastNotificationDate = new ChangeTracker<DateTime?>();
            IsUserHandled = new ChangeTracker<bool>();
            LastStatusEventId = new ChangeTracker<Guid?>();
            FirstReasonEventId = new ChangeTracker<Guid?>();
        }

        /// <summary>
        /// Идентификатор события
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Счетчик - сколько раз случалось событие
        /// </summary>
        public ChangeTracker<int> Count { get; }

        /// <summary>
        /// Сообщение
        /// </summary>
        public ChangeTracker<string> Message { get; }

        /// <summary>
        /// Важность события
        /// </summary>
        public ChangeTracker<EventImportance> Importance { get; }

        /// <summary>
        /// Дата завершения события (по местному времени аккаунта)
        /// </summary>
        public ChangeTracker<DateTime> EndDate { get; }

        /// <summary>
        /// Время ДО которого событие можно считать актуальным
        /// </summary>
        public ChangeTracker<DateTime> ActualDate { get; }

        public ChangeTracker<DateTime> StartDate { get; }

        /// <summary>
        /// Время последнего изменения события.
        /// </summary>
        public ChangeTracker<DateTime> LastUpdateDate { get; }

        /// <summary>
        /// Время последней обработки события для создания уведомлений
        /// </summary>
        public ChangeTracker<DateTime?> LastNotificationDate { get; }

        /// <summary>
        /// Событие обработано человеком
        /// </summary>
        public ChangeTracker<bool> IsUserHandled { get; }

        /// <summary>
        /// Ссылка на последний статус, для которого данное событие является причиной
        /// </summary>
        public ChangeTracker<Guid?> LastStatusEventId { get; }

        /// <summary>
        /// Ссылка на первую причину данного статуса
        /// </summary>
        public ChangeTracker<Guid?> FirstReasonEventId { get; }

    }
}