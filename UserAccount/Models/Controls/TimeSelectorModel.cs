using Microsoft.AspNetCore.Mvc.Rendering;

namespace Zidium.UserAccount.Models.Controls
{
    public class TimeSelectorModel
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public IHtmlHelper HtmlHelper { get; set; }

    }
}