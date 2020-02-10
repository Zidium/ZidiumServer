using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;

namespace Zidium.UserAccount.Models
{
    public class NotificationDetailsModel
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Адрес")]
        public string Address { get; set; }

        [Display(Name = "Событие")]
        public Event Event { get; set; }

        [Display(Name = "Получатель")]
        public User User { get; set; }

        [Display(Name = "Канал")]
        public SubscriptionChannel Channel { get; set; }

        [Display(Name = "Статус")]
        public NotificationStatus Status { get; set; }

        [Display(Name = "Текст ошибки")]
        public string SendError { get; set; }

        [Display(Name = "Дата создания")]
        public DateTime CreationDate { get; set; }

        [Display(Name = "Дата отправки")]
        public DateTime? SendDate { get; set; }

        [Display(Name = "Подписка")]
        public Subscription Subscription { get; set; }

        [Display(Name = "Сообщение")]
        public string Text { get; set; }

    }
}