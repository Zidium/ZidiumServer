using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Core.Api;

namespace Zidium.UserAccount.Models
{
    public class AddEventModel
    {
        public Guid ComponentId { get; set; }

        [Display(Name = "Тип")]
        [MaxLength(1000)]
        [Required(ErrorMessage = "Пожалуйста, укажите тип события")]
        public string EventType { get; set; }

        [Display(Name = "Сообщение")]
        [MaxLength(1000)]
        [DataType("TextArea")]
        public string Message { get; set; }

        [Display(Name = "Важность")]
        [Required(ErrorMessage = "Пожалуйста, укажите важность")]
        public EventImportance EventImportance { get; set; }

        [Display(Name = "Склейка")]
        public TimeSpan? JoinInterval { get; set; }

        public string DisplayName { get; set; }

        public string ReturnUrl { get; set; }
    }
}