using System.ComponentModel.DataAnnotations;

namespace Zidium.UserAccount.Models.DomainNamePaymentPeriodCheckModels
{
    public class EditSimpleModel : CheckSimpleBaseModel
    {
        [Required(ErrorMessage = "Пожалуйста, укажите доменное имя")]
        [Display(Name = "Доменное имя", Description = "Домен")]
        public string Domain { get; set; }

    }
}