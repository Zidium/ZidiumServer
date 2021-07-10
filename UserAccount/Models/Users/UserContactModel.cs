using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Storage;

namespace Zidium.UserAccount.Models
{
    /// <summary>
    /// Модель добавления или редактирования контакта пользователя
    /// </summary>
    public class UserContactModel
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        [Required]
        [Display(Name = "Тип")]
        public UserContactType Type { get; set; }

        [Required]
        [MaxLength(70)]
        [Display(Name = "Контакт")]
        public string Value { get; set; }

        public bool ModalMode { get; set; }

        public string ReturnUrl { get; set; }

    }
}