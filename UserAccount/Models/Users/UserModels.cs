using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Zidium.Storage;

namespace Zidium.UserAccount.Models
{
    /// <summary>
    /// Модель редактирования пользователя
    /// </summary>
    public class EditUserModel
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Логин")]
        [EmailAddress(ErrorMessage = "Это не похоже на Email")]
        [Remote("CheckExistingLogin", "Users", AdditionalFields = "Id")]
        public string Login { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Отображаемое имя")]
        public string DisplayName { get; set; }

        [MaxLength(50)]
        [Display(Name = "Должность")]
        public string Post { get; set; }

        [Required]
        [Display(Name = "Роль")]
        public Guid? RoleId { get; set; }

        [Display(Name = "Отправлять новости и полезную информацию")]
        public bool SendMeNews { get; set; }

        [Display(Name = "Часовой пояс")]
        public int TimeZoneOffsetMinutes { get; set; }

        public UserContactForRead[] Contacts { get; set; }

    }
}