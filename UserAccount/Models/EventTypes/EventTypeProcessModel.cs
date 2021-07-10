using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Api.Dto;

namespace Zidium.UserAccount.Models
{
    public class EventTypeProcessModel
    {
        public Guid EventTypeId { get; set; }

        [Display(Name = "Номер версии")]
        public string OldVersion { get; set; }

        [Display(Name = "Важность")]
        [Required]
        public EventImportance Importance { get; set; }

        [Display(Name = "Помечать обработанным")]
        public bool MarkAsProcessed { get; set; }

        public bool ModalMode { get; set; }

        public string ReturnUrl { get; set; }
    }
}