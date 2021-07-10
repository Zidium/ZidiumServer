using System;
using System.ComponentModel.DataAnnotations;

namespace Zidium.UserAccount.Models.Defects
{
    public class EditDefectModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string DefectCode { get; set; }

        [Required]
        public Guid? UserId { get; set; }

        public string Notes { get; set; }
    }
}