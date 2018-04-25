using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Zidium.UserAccount.Models
{
    public class ComponentTypeEditModel
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [MyRequired]
        [Display(Name = "Дружелюбное имя")]
        [StringLength(255)]
        public string DisplayName { get; set; }

        [MyRequired]
        [Display(Name = "Системное имя")]
        [StringLength(255)]
        [Remote("CheckSystemName", "ComponentTypes", AdditionalFields = "Id")]
        public string SystemName { get; set; }

        [Display(Name = "Системный?")]
        public bool IsCommon { get; set; }

        public bool IsDeleted { get; set; }
    }
}