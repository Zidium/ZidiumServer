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
        [Display(Name = "Логин")]
        [Remote("CheckNewLogin", "Users", AdditionalFields = "Id")]
        public string Login { get; set; }

        [MaxLength(255)]
        [Display(Name = "Отображаемое имя")]
        public string DisplayName { get; set; }

        [MaxLength(255)]
        [Display(Name = "EMail")]
        [EmailAddress(ErrorMessage = "Это не похоже на Email")]
        public string EMail { get; set; }

        [Required]
        [Display(Name = "Роль")]
        public Guid? RoleId { get; set; }

        [Display(Name = "Часовой пояс")]
        public int TimeZoneOffsetMinutes { get; set; }
    }
}