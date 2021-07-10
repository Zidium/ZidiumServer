using System.ComponentModel.DataAnnotations;

namespace Zidium.UserAccount.Models.HttpRequestCheckModels
{
    public class EditSimpleModel : CheckSimpleBaseModel
    {
        [Required(ErrorMessage = "Пожалуйста, укажите адрес сайта")]
        [Display(Name = "Сайт", Description = "Адрес сайта")]
        public string Url { get; set; }

    }
}