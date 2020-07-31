using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Storage;

namespace Zidium.UserAccount.Models
{
    public class EventTypeIgnoreModel
    {
        public Guid EventTypeId { get; set; }

        public bool ModalMode { get; set; }

        public string ReturnUrl { get; set; }

        [Display(Name = "Важность")]
        [MyRequired]
        public EventImportance Importance { get; set; }

        [Display(Name = "Помечать обработанным")]
        public bool MarkAsProcessed { get; set; }

    }
}