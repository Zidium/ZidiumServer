using System;
using System.ComponentModel.DataAnnotations;

namespace Zidium.UserAccount.Models
{
    public class SetPasswordModel
    {
        public Guid TokenId { get; set; }

        public Guid AccountId { get; set; }

        [Display(Name = "Пользователь")]
        public string UserName { get; set; }

        [Display(Name = "Новый пароль")]
        [Required(ErrorMessage = "Пожалуйста, введите пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Пароль ещё раз")]
        [Required(ErrorMessage = "Пожалуйста, введите пароль ещё раз")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public string PasswordConfirmation { get; set; }
    }
}