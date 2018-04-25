using System;
using System.ComponentModel.DataAnnotations;
using Zidium.Core.Api;

namespace Zidium.UserAccount.Models
{
    public class UnitTestSetResultModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Пожалуйста, укажите результат")]
        [Display(Name = "Результат")]
        public UnitTestResult Result { get; set; }

        [Display(Name = "Сообщение")]
        [MaxLength(1000)]
        [DataType("TextArea")]
        public string Message { get; set; }

        [Required(ErrorMessage = "Пожалуйста, укажите интервал актуальности")]
        [Display(Name = "Актуальность")]
        public TimeSpan ActualInterval { get; set; }

        public string DisplayName { get; set; }
    }
}