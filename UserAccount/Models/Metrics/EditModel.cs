using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Zidium.Core.AccountsDb;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Models.Metrics
{
    public class EditModel
    {
        [Display(Name = "Id")]
        public Guid? Id { get; set; }

        [Display(Name = "Тип метрики")]
        [Required(ErrorMessage = "Выберите тип метрики")]
        public Guid? MetricTypeId { get; set; }

        [Display(Name = "Красный", Description = "Например, value < 10")]
        [StringLength(255)]
        [DataType("TextArea")]
        [AllowHtml]
        public string ConditionRed { get; set; }

        [Display(Name = "Жёлтый", Description = "Например, value < 100")]
        [StringLength(255)]
        [DataType("TextArea")]
        [AllowHtml]
        public string ConditionYellow { get; set; }

        [Display(Name = "Зелёный", Description = "Например, value < 1000")]
        [StringLength(255)]
        [DataType("TextArea")]
        [AllowHtml]
        public string ConditionGreen { get; set; }

        [Display(Name = "Иначе")]
        public ColorStatusSelectorValue ElseColor { get; set; }

        [Display(Name = "Интервал актуальности")]
        public TimeSpan? ActualTime { get; set; }

        [Display(Name = "Цвет, если нет сигнала")]
        public ColorStatusSelectorValue NoSignalColor { get; set; }

        [Display(Name = "Компонент")]
        [DataType("Component")]
        [Required(ErrorMessage = "Выберите компонент")]
        public Guid? ComponentId { get; set; }

        [Display(Name = "Тип метрики")]
        public MetricType MetricType { get; set; }

        public Metric Metric { get; set; }
    }
}