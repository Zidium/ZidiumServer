using Microsoft.AspNetCore.Html;

namespace Zidium.UserAccount.Models.Controls
{
    public class FormGroupModel
    {
        public string Name { get; set; }

        public string CssClass { get; set; }

        public IHtmlContent Label { get; set; }

        public IHtmlContent Tooltip { get; set; }

        public IHtmlContent Control { get; set; }

        public string ControlWidth { get; set; }
    }
}