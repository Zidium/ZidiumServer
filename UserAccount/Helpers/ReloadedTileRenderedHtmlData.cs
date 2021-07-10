using Microsoft.AspNetCore.Html;

namespace Zidium.UserAccount.Helpers
{
    public class ReloadedTileRenderedHtmlData
    {
        public string Url { get; set; }
        public IHtmlContent Html { get; set; }
        public string Hash { get; set; }
    }
}