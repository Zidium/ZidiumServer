using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Storage;

namespace Zidium.UserAccount.Models
{
    public class LogFiltersModel
    {
        [Display(Name = "Компонент")]
        public Guid? ComponentId { get; set; }

        [Display(Name = "Дата")]
        public DateTime? Date { get; set; }

        [Display(Name = "Уровень детализации")]
        public LogLevel? LogLevel { get; set; }

        [Display(Name = "Контекст")]
        public string Context { get; set; }

        [Display(Name = "Поиск по тексту")]
        public string Text { get; set; }

        public Guid? Id { get; set; }
    }
}