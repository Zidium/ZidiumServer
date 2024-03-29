﻿using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Zidium.Api.Dto;
using Zidium.Storage;

namespace Zidium.UserAccount.Models
{
    public class EventTypeEditModel
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "Дружелюбное имя")]
        [StringLength(255)]
        [DataType("TextArea")]
        public string DisplayName { get; set; }

        [Required]
        [Display(Name = "Системное имя")]
        [StringLength(255)]
        [Remote("CheckSystemName", "EventTypes", AdditionalFields = "Id")]
        [DataType("TextArea")]
        public string SystemName { get; set; }

        [Display(Name = "Тип компонента")]
        public ComponentTypeForRead ComponentType { get; set; }

        [Required]
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