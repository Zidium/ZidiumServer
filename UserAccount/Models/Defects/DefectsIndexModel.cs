using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.Defects
{
    public class DefectsIndexModel
    {
        public enum ShowModeEnum
        {
            InWork = 1,
            Testing = 2,
            Closed = 3,
            All = 4
        }

        [Display(Name = "Ответственный")]
        //public bool OnlyMy { get; set; }
        public Guid? UserId { get; set; }

        [Display(Name = "Статус")]
        public ShowModeEnum ShowMode { get; set; }

        [Display(Name = "Компонент")]
        public Guid? ComponentId { get; set; }

        [Display(Name = "Текст")]
        public string Title { get; set; }

        public DefectsIndexItemModel[] Items { get; set; }

        public int TotalCount { get; set; }

        public int InWorkCount { get; set; }

        public int TestingCount { get; set; }

        public int ClosedCount { get; set; }

        public static readonly DefectStatus[] InWorkStatuses = new[]
        {
            DefectStatus.Open,
            DefectStatus.Reopened,
            DefectStatus.InProgress
        };
    }
}