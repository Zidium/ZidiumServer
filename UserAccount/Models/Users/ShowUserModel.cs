using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Models
{
    public class ShowUserModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Display(Name = "Отображаемое имя")]
        public string DisplayName { get; set; }

        [Display(Name = "Роль")]
        public string Role { get; set; }

        [Display(Name = "Отправлять новости и полезную информацию")]
        public bool SendMeNews { get; set; }

        [Display(Name = "Часовой пояс")]
        public string TimeZone { get; set; }

        public List<UserContact> Contacts { get; set; }

    }
}