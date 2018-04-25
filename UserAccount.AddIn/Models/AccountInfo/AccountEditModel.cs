using System;
using System.ComponentModel.DataAnnotations;

namespace Zidium.UserAccount.Models
{
    public class AccountEditModel
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Секретный ключ")]
        public string SecretKey { get; set; }
    }
}