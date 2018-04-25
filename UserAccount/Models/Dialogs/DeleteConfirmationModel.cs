namespace Zidium.UserAccount.Models
{
    public class DeleteConfirmationModel
    {
        public string Id { get; set; }
        public bool ModalMode { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string ReturnUrl { get; set; }
        public string AjaxUpdateTargetId { get; set; }
        public string OnAjaxSuccess { get; set; }
    }
}