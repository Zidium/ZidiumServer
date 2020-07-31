using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Models.MetricTypes
{
    public class EditModel
    {
        [Display(Name = "Id")]
        public Guid? Id { get; set; }

        [Display(Name = "Системное название")]
        [Required(ErrorMessage = "Пожалуйста, заполните название")]
        [StringLength(255)]
        [Remote("CheckName", "MetricTypes", AdditionalFields = "Id")]
        public string SystemName { get; set; }

        [Display(Name = "Отображаемое название")]
        [StringLength(255)]
        public string DisplayName { get; set; }

        [Display(Name = "Время актуальности")]
        public TimeSpan? ActualTime { get; set; }

        [Display(Name = "Цвет, если нет сигнала")]
        public ColorStatusSelectorValue NoSignalColor { get; set; }

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

        public bool ModalMode { get; set; }
    }
}