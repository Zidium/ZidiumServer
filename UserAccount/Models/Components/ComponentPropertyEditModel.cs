using System;
using System.ComponentModel.DataAnnotations;

namespace Zidium.UserAccount.Models
{
    public class ComponentPropertyEditModel
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Название")]
        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Display(Name = "Значение")]
        [StringLength(255)]
        public string Value { get; set; }

        [Display(Name = "Тип данных")]
        [Required]
        public Api.Dto.DataType DataType { get; set; }

        public bool ModalMode { get; set; }

        public string ReturnUrl { get; set; }

        public Guid ComponentId { get; set; }
    }
}