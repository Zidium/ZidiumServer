using System;
using System.ComponentModel.DataAnnotations;

namespace Zidium.UserAccount.Models
{
    public abstract class CheckSimpleBaseModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Пожалуйста, укажите период проверки")]
        [Display(Name = "Период")]
        public TimeSpan Period { get; set; }

        public Guid? ComponentId { get; set; }

    }
}