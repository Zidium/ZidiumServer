using System.IO;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Zidium.Core.Common;

namespace Zidium.UserAccount.Helpers
{
    public static class ReloadedTileHelper
    {
        public static ReloadedTileRenderedHtmlData GetRenderdHtmlData(string url, IHtmlHelper htmlHelper)
        {
            var urlData = ControllerActionUrlParser.Parse(url);

            var mvcHtml = htmlHelper.Action(urlData.ActionName, urlData.ControllerName, urlData.RouteValues);

            using var writer = new StringWriter();
            mvcHtml.WriteTo(writer, HtmlEncoder.Default);
            var html = writer.ToString();

            var hash = HashHelper.GetInt64(html).ToString();

            return new ReloadedTileRenderedHtmlData()
            {
                Url = url,
                Html = mvcHtml,
                Hash = hash
            };
        }

        public static IHtmlContent PartialReloadedTile(this IHtmlHelper htmlHelper, string url)
        {
            return htmlHelper.Action("Render", "ReloadedTiles", new { url });
        }
    }
}