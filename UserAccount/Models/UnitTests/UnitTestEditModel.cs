using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Storage;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Models
{
    public class UnitTestEditModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Компонент")]
        [MyRequired]
        [DataType("ComponentTree")]
        public Guid ComponentId { get; set; }

        [Display(Name = "Тип")]
        public UnitTestTypeForRead UnitTestType { get; set; }

        [Display(Name = "Период, сек")]
        public int? PeriodSeconds { get; set; }

        [Display(Name = "Интервал актуальности")]
        public TimeSpan? ActualTime { get; set; }

        public TimeSpan ActualTimeDefault { get; set; }

        [Display(Name = "Цвет, если нет сигнала")]
        public ColorStatusSelectorValue NoSignalColor { get; set; }

        public ObjectColor NoSignalColorDefault { get; set; }

        [Display(Name = "Название")]
        [MyRequired]
        [StringLength(255)]
        public string DisplayName { get; set; }

        public bool IsDeleted { get; set; }

        [Display(Name = "Дата проверки")]
        public DateTime? Date { get; set; }

        [Display(Name = "Результат")]
        public MonitoringStatus Status { get; set; }

        [Display(Name = "Сообщение")]
        public string Message { get; set; }

        public UnitTestBreadCrumbsModel UnitTestBreadCrumbs { get; set; }

    }
}