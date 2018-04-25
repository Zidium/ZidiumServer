using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Models
{
    public class UnitTestTypeEditModel
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Дружелюбное имя")]
        [MyRequired]
        [StringLength(255)]
        public string DisplayName { get; set; }

        [Display(Name = "Системное имя")]
        [MyRequired]
        [StringLength(255)]
        [Remote("CheckSystemName", "UnitTestTypes", AdditionalFields = "Id")]
        public string SystemName { get; set; }

        [Display(Name = "Системный?")]
        public bool IsSystem { get; set; }

        [Display(Name = "Интервал актуальности")]
        public TimeSpan? ActualTime { get; set; }

        [Display(Name = "Цвет, если нет сигнала")]
        public ColorStatusSelectorValue NoSignalColor { get; set; }

        public bool IsDeleted { get; set; }
    }
}