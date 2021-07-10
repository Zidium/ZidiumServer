using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Storage;

namespace Zidium.UserAccount.Models
{
    public class SendNotificationModel
    {
        [DataType("User")]
        [Display(Name = "Получатель")]
        public Guid UserId { get; set; }

        [Display(Name = "Канал")]
        public SubscriptionChannel Channel { get; set; }

    }
}