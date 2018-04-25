using System;
using Zidium.Core.Api;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Информация о последнем уведомлении о статусе компонента на определеный адрес
    /// Уникален по ключу = {ComponentId, Address, Type}
    /// </summary>
    public class LastComponentNotification
    {
        public Guid Id { get; set; }

        public Guid ComponentId { get; set; }

        public virtual Component Component { get; set; }

        public string Address { get; set; }

        public NotificationType Type { get; set; }

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

        public virtual Event Event { get; set; }

        /// <summary>
        /// Ссылка на уведомление
        /// </summary>
        public Guid NotificationId { get; set; }

        public virtual Notification Notification { get; set; }

        /// <summary>
        /// Обновляет данные о последнем уведомлении
        /// </summary>
        public void Update(Notification notification, EventImportance importance)
        {
            if (notification == null)
            {
                throw new ArgumentNullException("notification");
            }
            Address = notification.Address;
            Type = notification.Type;
            EventImportance = importance;
            CreateDate = notification.CreationDate;
            EventId = notification.EventId;
            NotificationId = notification.Id;
        }
    }
}
