using System.ComponentModel.DataAnnotations;

namespace Zidium.UserAccount.Models
{
    public class LogonModel
    {
        [Required]
        [Display(Name = "Email")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        public string ErrorMessage { get; set; }
    }
}