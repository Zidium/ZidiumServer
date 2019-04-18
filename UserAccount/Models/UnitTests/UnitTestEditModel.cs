using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Models
{
    public class UnitTestEditModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Компонент")]
        public Component Component { get; set; }

        [Display(Name = "Компонент")]
        [MyRequired]
        [DataType("ComponentTree")]
        public Guid ComponentId { get; set; }

        [Display(Name = "Тип")]
        public UnitTestType UnitTestType { get; set; }

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

        public UnitTest UnitTest { get; set; }

        public void InitReadOnlyValues(UnitTest unitTest)
        {
            UnitTest = unitTest;
            UnitTestType = unitTest.Type;
            Id = unitTest.Id;
            NoSignalColorDefault = unitTest.Type.NoSignalColor ?? ObjectColor.Red;
            ActualTimeDefault = TimeSpanHelper.FromSeconds(unitTest.Type.ActualTimeSecs) ?? UnitTestHelper.GetDefaultActualTime();
        }
    }
}