using System.ComponentModel.DataAnnotations;

namespace Zidium.UserAccount.Models.Home
{
    public class QuickSetMobilePhoneModel
    {
        [Required(ErrorMessage = "Пожалуйста, заполните телефон")]
        [Display(Name = "Мобильный")]
        public string Phone { get; set; }

        public string ReturnUrl { get; set; }
    }
}