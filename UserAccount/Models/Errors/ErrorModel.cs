using System;

namespace Zidium.UserAccount.Models
{
    public class ErrorModel
    {
        public Exception Exception { get; set; }

        public bool IsAjax { get; set; }
    }
}