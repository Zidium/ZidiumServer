using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Models
{
    /// <summary>
    /// Модель редактирования пользователя
    /// </summary>
    public class EditUserModel
    {
        public Guid Id { get; set; }

        [MyRequired]
        [MaxLength(255)]
        [Display(Name = "Логин")]
        [EmailAddress(ErrorMessage = "Это не похоже на Email")]
        [Remote("CheckExistingLogin", "Users", AdditionalFields = "Id")]
        public string Login { get; set; }

        //[MaxLength(100)]
        //[Display(Name = "Фамилия")]
        //public string LastName { get; set; }

        //[MaxLength(100)]
        //[Display(Name = "Имя")]
        //public string FirstName { get; set; }

        //[MaxLength(100)]
        //[Display(Name = "Отчество")]
        //public string MiddleName { get; set; }

        [MyRequired]
        [MaxLength(100)]
        [Display(Name = "Отображаемое имя")]
        public string DisplayName { get; set; }

        [MaxLength(50)]
        [Display(Name = "Должность")]
        public string Post { get; set; }

        [MyRequired]
        [Display(Name = "Роль")]
        public Guid? RoleId { get; set; }

        [Display(Name = "Отправлять новости и полезную информацию")]
        public bool SendMeNews { get; set; }

        [Display(Name = "Часовой пояс")]
        public int TimeZoneOffsetMinutes { get; set; }

        public List<UserContact> Contacts { get; set; }

    }
}