using System.Web.Mvc;

namespace Zidium.UserAccount.Models.Controls
{
    public class FormGroupModel
    {
        public string Name { get; set; }

        public string CssClass { get; set; }

        public MvcHtmlString Label { get; set; }

        public MvcHtmlString Tooltip { get; set; }

        public MvcHtmlString Control { get; set; }

        public string ControlWidth { get; set; }
    }
}