using System;
using System.ComponentModel.DataAnnotations;

namespace Zidium.UserAccount.Models
{
    public class DisableDialogAjaxModel
    {
        public Guid Id { get; set; }

        public string Message { get; set; }

        public DisableInterval Interval { get; set; }

        public DateTime? Date { get; set; }

        [Display(Name = "Комментарий")]
        public string Comment { get; set; }

        public enum DisableInterval
        {
            Hour, Day, Week, Forever, Custom
        }
    }
}