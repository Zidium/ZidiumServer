using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Api.Dto;

namespace Zidium.UserAccount.Models
{
    public class EventTypeIgnoreModel
    {
        public Guid EventTypeId { get; set; }

        public bool ModalMode { get; set; }

        public string ReturnUrl { get; set; }

        [Display(Name = "Важность")]
        [Required]
        public EventImportance Importance { get; set; }

        [Display(Name = "Помечать обработанным")]
        public bool MarkAsProcessed { get; set; }

    }
}