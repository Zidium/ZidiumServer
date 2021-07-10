using System;
using System.ComponentModel.DataAnnotations;

namespace Zidium.UserAccount.Models.Components
{
    /// <summary>
    /// Данные для создания нового компонента в папке
    /// </summary>
    public class CreateNewInFolderModel
    {
        public Guid? ComponentId { get; set; }

        [Required]
        [Display(Name = "Системное имя")]
        public string SystemName { get; set; }

        [Display(Name = "Отображаемое имя")]
        public string DisplayName { get; set; }

        [Required]
        [Display(Name = "Тип компонента")]
        public Guid? ComponentTypeId { get; set; }

        public string FolderSystemName { get; set; }

        public string FolderDisplayName { get; set; }

        public string ErrorMessage { get; set; }
    }
}