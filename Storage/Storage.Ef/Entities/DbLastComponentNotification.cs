using System;
using Zidium.Api.Dto;

namespace Zidium.Storage.Ef
{
    /// <summary>
    /// Информация о последнем уведомлении о статусе компонента на определеный адрес
    /// Уникален по ключу = {ComponentId, Address, Type}
    /// </summary>
    public class DbLastComponentNotification
    {
        public Guid Id { get; set; }

        public Guid ComponentId { get; set; }

        public virtual DbComponent Component { get; set; }

        public string Address { get; set; }

        public SubscriptionChannel Type { get; set; }

        /// <summary>
        /// Важность статуса (цвет компонента), которое было в уведомлении
        /// </summary>
        public EventImportance EventImportance { get; set; }

        /// <summary>
        /// Дата и время создания последнего уведомления
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Ссылка на статус компонента (событие)
        /// </summary>
        public Guid EventId { get; set; }

        public virtual DbEvent Event { get; set; }

        /// <summary>
        /// Ссылка на уведомление
        /// </summary>
        public Guid NotificationId { get; set; }

        public virtual DbNotification Notification { get; set; }

    }
}
