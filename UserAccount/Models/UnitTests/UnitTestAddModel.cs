using System;
using System.ComponentModel.DataAnnotations;

namespace Zidium.UserAccount.Models
{
    public class UnitTestAddModel
    {
        [Display(Name = "Тип проверки")]
        [Required]
        [DataType("UnitTestType")]
        public Guid UnitTestTypeId { get; set; }

        [Display(Name = "Компонент")]
        [Required]
        [DataType("ComponentTree")]
        public Guid? ComponentId { get; set; }

        [Display(Name = "Название")]
        [Required]
        [StringLength(255)]
        public string DisplayName { get; set; }
    }
}