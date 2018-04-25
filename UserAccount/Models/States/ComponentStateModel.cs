using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Core.Api;
using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Models
{
    public class ComponentStateModel
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Компонент")]
        public Component Component { get; set; }

        [Display(Name = "Дата начала")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Дата обновления")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Дата актуальности")]
        public DateTime ActualDate { get; set; }

        [Display(Name = "Длительность")]
        public TimeSpan Duration { get; set; }

        [Display(Name = "Количество обновлений")]
        public int Count { get; set; }

        [Display(Name = "Статус")]
        public MonitoringStatus Status { get; set; }

        [Display(Name = "Исходное событие")]
        public Event LastEvent { get; set; }

        [Display(Name = "Статусное событие")]
        public Event StatusEvent { get; set; }

        [Display(Name = "Описание")]
        public string Message { get; set; }

    }
}