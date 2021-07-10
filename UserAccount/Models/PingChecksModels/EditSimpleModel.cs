using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Zidium.UserAccount.Models.PingChecksModels
{
    public class EditSimpleModel : CheckSimpleBaseModel
    {
        [Required(ErrorMessage = "Пожалуйста, укажите IP или домен")]
        [Display(Name = "Сервер", Description = "IP или домен")]
        [Remote("CheckHost", "PingChecks")]
        public string Host { get; set; }

    }
}