using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;

namespace Zidium.UserAccount.Models
{
    public class EventTypeEditModel
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [MyRequired]
        [Display(Name = "Дружелюбное имя")]
        [StringLength(255)]
        [DataType("TextArea")]
        public string DisplayName { get; set; }

        [MyRequired]
        [Display(Name = "Системное имя")]
        [StringLength(255)]
        [Remote("CheckSystemName", "EventTypes", AdditionalFields = "Id")]
        [DataType("TextArea")]
        public string SystemName { get; set; }

        [Display(Name = "Тип компонента")]
        public ComponentType ComponentType { get; set; }
        
        [MyRequired]
        [Display(Name = "Категория")]
        public EventCategory Category { get; set; }

        [Display(Name = "Интервал склейки")]
        public TimeSpan? JoinInterval { get; set; }

        [Display(Name = "Номер старой версии")]
        public string OldVersion { get; set; }

        [Display(Name = "Важность")]
        public EventImportance? ImportanceForOld { get; set; }
        
        [Display(Name = "Важность")]
        public EventImportance? ImportanceForNew { get; set; }
        
        public bool IsSystem { get; set; }

        public bool IsDeleted { get; set; }

    }
}