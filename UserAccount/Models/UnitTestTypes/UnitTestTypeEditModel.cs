using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Zidium.Api.Dto;
using Zidium.Core.Common.Helpers;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Models
{
    public class UnitTestTypeEditModel
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Дружелюбное имя")]
        [Required]
        [StringLength(255)]
        public string DisplayName { get; set; }

        [Display(Name = "Системное имя")]
        [Required]
        [StringLength(255)]
        [Remote("CheckSystemName", "UnitTestTypes", AdditionalFields = "Id")]
        public string SystemName { get; set; }

        [Display(Name = "Системный?")]
        public bool IsSystem { get; set; }

        [Display(Name = "Интервал актуальности")]
        public TimeSpan? ActualTime { get; set; }

        public TimeSpan ActualTimeDefault { get; set; }

        [Display(Name = "Цвет, если нет сигнала")]
        public ColorStatusSelectorValue NoSignalColor { get; set; }

        public ObjectColor NoSignalColorDefault { get; set; }

        public bool IsDeleted { get; set; }

        public void InitReadOnlyData()
        {
            NoSignalColorDefault = ObjectColor.Red;
            ActualTimeDefault = UnitTestHelper.GetDefaultActualTime();
        }
    }
}