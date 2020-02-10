using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Zidium.UserAccount.Models.VirusTotal
{
    public class EditSimpleModel : CheckSimpleBaseModel
    {
        [Required(ErrorMessage = "Пожалуйста, укажите Url ресурса для проверки")]
        [Display(Name = "Url", Description = "Url ресурса проверки")]
        public string Url { get; set; }

        [Required(ErrorMessage = "Пожалуйста, укажите ApiKey из вашего аккаунта VirusTotal")]
        [Display(Name = "ApiKey", Description = "ApiKey вашего аккаунта VirusTotal")]
        public string ApiKey { get; set; }

        public string CommonWebsiteUrl { get; set; }
    }
}