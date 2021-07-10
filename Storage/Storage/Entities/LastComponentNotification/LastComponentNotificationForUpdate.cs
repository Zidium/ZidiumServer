using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    /// <summary>
    /// Информация о последнем уведомлении о статусе компонента на определеный адрес
    /// Уникален по ключу = {ComponentId, Address, Type}
    /// </summary>
    public class LastComponentNotificationForUpdate
    {
        public LastComponentNotificationForUpdate(Guid id)
        {
            Id = id;
            EventImportance = new ChangeTracker<EventImportance>();
            CreateDate = new ChangeTracker<DateTime>();
            EventId = new ChangeTracker<Guid>();
            NotificationId = new ChangeTracker<Guid>();
        }

        public Guid Id { get; }

        /// <summary>
        /// Важность статуса (цвет компонента), которое было в уведомлении
        /// </summary>
        public ChangeTracker<EventImportance> EventImportance { get; }

        /// <summary>
        /// Дата и время создания последнего уведомления
        /// </summary>
        public ChangeTracker<DateTime> CreateDate { get; }

        /// <summary>
        /// Ссылка на статус компонента (событие)
        /// </summary>
        public ChangeTracker<Guid> EventId { get; }

        /// <summary>
        /// Ссылка на уведомление
        /// </summary>
        public ChangeTracker<Guid> NotificationId { get; }

    }
}