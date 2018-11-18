using System.Web.Mvc;

namespace Zidium.UserAccount.Helpers
{
    public class ReloadedTileRenderedHtmlData
    {
        public string Url { get; set; }
        public MvcHtmlString Html { get; set; }
        public string Hash { get; set; }
    }
}