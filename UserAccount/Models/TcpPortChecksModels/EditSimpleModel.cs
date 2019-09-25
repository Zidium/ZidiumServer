using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Zidium.UserAccount.Models.TcpPortChecksModels
{
    public class EditSimpleModel : CheckSimpleBaseModel
    {
        [Required(ErrorMessage = "Пожалуйста, укажите IP или домен")]
        [Display(Name = "Сервер", Description = "IP или домен")]
        [Remote("CheckHost", "TcpPortChecks")]
        public string Host { get; set; }

        [Required(ErrorMessage = "Пожалуйста, укажите порт")]
        [Display(Name = "Порт")]
        [Remote("CheckPort", "TcpPortChecks")]
        public int? Port { get; set; }

        [Display(Name = "Порт должен быть открыт")]
        public bool Opened { get; set; }

        public EditSimpleModel()
        {
            Opened = true;
        }
    }
}