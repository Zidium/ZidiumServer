using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Core.Api;

namespace Zidium.UserAccount.Models
{
    public class AddLogModel
    {
        public Guid ComponentId { get; set; }

        [Required(ErrorMessage = "Пожалуйста, укажите дату и время")]
        [Display(Name = "Дата и время")]
        [DataType("Date")]
        public DateTime LogDate { get; set; }

        [Required(ErrorMessage = "Пожалуйста, укажите уровень")]
        [Display(Name = "Уровень")]
        public LogLevel LogLevel { get; set; }

        [Display(Name = "Сообщение")]
        [Required(ErrorMessage = "Пожалуйста, укажите сообщение")]
        [MaxLength(8000)]
        [DataType("TextArea")]
        public string Message { get; set; }

        public string DisplayName { get; set; }
    }
}