using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Zidium.UserAccount.Models
{
    /// <summary>
    /// Модель добавления пользователя
    /// </summary>
    public class AddUserModel
    {
        [Required]
        [MaxLength(255)]
        [Display(Name = "EMail", Description = "Будет использован как логин")]
        [EmailAddress(ErrorMessage = "Это не похоже на Email")]
        [Remote("CheckNewLogin", "Users", AdditionalFields = "Id")]
        public string Login { get; set; }

        [MaxLength(100)]
        [Display(Name = "Отображаемое имя")]
        [Required]
        public string DisplayName { get; set; }

        [MaxLength(100)]
        [Display(Name = "Должность")]
        public string Post { get; set; }

        [Required]
        [Display(Name = "Роль")]
        public Guid? RoleId { get; set; }

        [Display(Name = "Часовой пояс")]
        public int TimeZoneOffsetMinutes { get; set; }
    }
}