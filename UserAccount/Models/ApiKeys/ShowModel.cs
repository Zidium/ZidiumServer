using System;
using System.ComponentModel.DataAnnotations;

namespace Zidium.UserAccount.Models.ApiKeys
{
    public class ShowModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Ключ доступа")]
        public string Value { get; set; }

        public bool ModalMode { get; set; }

        public string ReturnUrl { get; set; }
    }
}
