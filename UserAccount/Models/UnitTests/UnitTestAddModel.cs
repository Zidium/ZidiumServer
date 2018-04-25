using System;
using System.ComponentModel.DataAnnotations;

namespace Zidium.UserAccount.Models
{
    public class UnitTestAddModel
    {
        [Display(Name = "Тип проверки")]
        [MyRequired]
        [DataType("UnitTestType")]
        public Guid UnitTestTypeId { get; set; }

        [Display(Name = "Компонент")]
        [MyRequired]
        [DataType("ComponentTree")]
        public Guid? ComponentId { get; set; }

        [Display(Name = "Название")]
        [MyRequired]
        [StringLength(255)]
        public string DisplayName { get; set; }
    }
}