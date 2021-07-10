using System.ComponentModel.DataAnnotations;

namespace Zidium.UserAccount.Models
{
    public class LostPasswordModel
    {
        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Пожалуйста, укажите логин")]
        public string Login { get; set; }
    }
}