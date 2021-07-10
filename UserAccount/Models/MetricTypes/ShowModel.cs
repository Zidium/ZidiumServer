using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Api.Dto;

namespace Zidium.UserAccount.Models.MetricTypes
{
    public class ShowModel
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Системное название")]
        public string SystemName { get; set; }

        [Display(Name = "Отображаемое название")]
        public string DisplayName { get; set; }

        [Display(Name = "Цвет, если нет сигнала")]
        public ObjectColor? NoSignalColor { get; set; }

        [Display(Name = "Время актуальности")]
        public TimeSpan? ActualTime { get; set; }

        [Display(Name = "Красный")]
        public string ConditionRed { get; set; }

        [Display(Name = "Жёлтый")]
        public string ConditionYellow { get; set; }

        [Display(Name = "Зелёный")]
        public string ConditionGreen { get; set; }

        [Display(Name = "Иначе")]
        public ObjectColor? ElseColor { get; set; }

        public bool ModalMode { get; set; }
    }
}