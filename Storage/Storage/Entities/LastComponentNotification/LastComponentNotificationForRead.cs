using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    /// <summary>
    /// Информация о последнем уведомлении о статусе компонента на определеный адрес
    /// Уникален по ключу = {ComponentId, Address, Type}
    /// </summary>
    public class LastComponentNotificationForRead
    {
        public LastComponentNotificationForRead(
            Guid id, 
            Guid componentId, 
            string address, 
            SubscriptionChannel type, 
            EventImportance eventImportance, 
            DateTime createDate, 
            Guid eventId, 
            Guid notificationId)
        {
            Id = id;
            ComponentId = componentId;
            Address = address;
            Type = type;
            EventImportance = eventImportance;
            CreateDate = createDate;
            EventId = eventId;
            NotificationId = notificationId;
        }

        public Guid Id { get; }

        public Guid ComponentId { get; }

        public string Address { get; }

        public SubscriptionChannel Type { get; }

        /// <summary>
        /// Важность статуса (цвет компонента), которое было в уведомлении
        /// </summary>
        public EventImportance EventImportance { get; }

        /// <summary>
        /// Дата и время создания последнего уведомления
        /// </summary>
        public DateTime CreateDate { get; }

        /// <summary>
        /// Ссылка на статус компонента (событие)
        /// </summary>
        public Guid EventId { get; }

        /// <summary>
        /// Ссылка на уведомление
        /// </summary>
        public Guid NotificationId { get; }

        public LastComponentNotificationForUpdate GetForUpdate()
        {
            return new LastComponentNotificationForUpdate(Id);
        }

    }
}
