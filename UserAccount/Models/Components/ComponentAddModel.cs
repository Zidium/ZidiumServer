using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Zidium.Storage;

namespace Zidium.UserAccount.Models
{
    public class ComponentAddModel
    {
        public ComponentForRead Component { get; set; }

        [Display(Name = "Id")]
        public Guid Id { get; set; }

        public bool CanEditParentId { get; set; }

        [Display(Name = "Дружелюбное имя")]
        [Required]
        [StringLength(255)]
        public string DisplayName { get; set; }

        [Display(Name = "Системное имя")]
        [StringLength(255)]
        [Remote("CheckSystemName", "Components", AdditionalFields = "Id,ParentId")]
        public string SystemName { get; set; }

        [Display(Name = "Родитель")]
        [Required]
        [DataType("ComponentTree")]
        public Guid ParentId { get; set; }

        public Component Parent { get; set; }

        [Display(Name = "Тип")]
        [Required]
        [DataType("ComponentType")]
        public Guid ComponentTypeId { get; set; }

        public ComponentTypeForRead ComponentType { get; set; }

        [Display(Name = "Версия")]
        [StringLength(255)]
        public string Version { get; set; }

        public bool IsDeleted { get; set; }

        public ComponentPropertyForRead[] Properties { get; set; }

    }
}