﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Zidium.UserAccount.Models.ApiKeys
{
    public class AddModel
    {
        [Required]
        [MaxLength(255)]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Ключ доступа")]
        public string Value { get; set; }

        [Display(Name = "Для пользователя")]
        public Guid? UserId { get; set; }

        public bool ModalMode { get; set; }

        public string ReturnUrl { get; set; }
    }
}
