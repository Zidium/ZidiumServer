using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    public class LastComponentNotificationForAdd
    {
        public Guid Id;

        public Guid ComponentId;

        public string Address;

        public SubscriptionChannel Type;

        /// <summary>
        /// Важность статуса (цвет компонента), которое было в уведомлении
        /// </summary>
        public EventImportance EventImportance;

        /// <summary>
        /// Дата и время создания последнего уведомления
        /// </summary>
        public DateTime CreateDate;

        /// <summary>
        /// Ссылка на статус компонента (событие)
        /// </summary>
        public Guid EventId;

        /// <summary>
        /// Ссылка на уведомление
        /// </summary>
        public Guid NotificationId;
    }
}
