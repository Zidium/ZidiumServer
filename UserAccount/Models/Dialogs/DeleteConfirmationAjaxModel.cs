using System.Web.Mvc;

namespace Zidium.UserAccount.Models
{
    public class DeleteConfirmationAjaxModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string OnAjaxSuccess { get; set; }
    }
}