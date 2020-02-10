using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Core.Api;

namespace Zidium.UserAccount.Models
{
    public class SendNotificationModel
    {
        [DataType("User")]
        [Display(Name = "Получатель")]
        public Guid UserId { get; set; }

        [Display(Name = "Канал")]
        public SubscriptionChannel Channel { get; set; }

        public string CommonWebsiteUrl { get; set; }

    }
}