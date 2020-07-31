using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Zidium.Storage;

namespace Zidium.UserAccount.Models
{
    public class ComponentEditModel
    {
        public ComponentForRead Component { get; set; }

        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Дружелюбное имя")]
        [MyRequired]
        [StringLength(255)]
        [AllowHtml]
        public string DisplayName { get; set; }

        [Display(Name = "Системное имя")]
        [MyRequired]
        [StringLength(255)]
        [AllowHtml]
        [Remote("CheckSystemName", "Components", AdditionalFields = "Id,ParentId")]
        public string SystemName { get; set; }

        [Display(Name = "Родитель")]
        [MyRequired]
        [DataType("ComponentTree")]
        public Guid? ParentId { get; set; }

        public ComponentForRead Parent { get; set; }

        [Display(Name = "Тип")]
        [MyRequired]
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