using System.ComponentModel.DataAnnotations;

namespace Zidium.UserAccount.Models.SslCertificateExpirationDateChecksModels
{
    public class EditSimpleModel : CheckSimpleBaseModel
    {
        [Required(ErrorMessage = "Пожалуйста, укажите адрес сайта")]
        [Display(Name = "Сайт", Description = "Адрес сайта")]
        public string Url { get; set; }

    }
}