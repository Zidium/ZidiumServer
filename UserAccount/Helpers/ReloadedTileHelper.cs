using System;
using System.IO;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Zidium.Core.Common;

namespace Zidium.UserAccount.Helpers
{
    public static class ReloadedTileHelper
    {
        public static ReloadedTileRenderedHtmlData GetRenderdHtmlData(string url, HtmlHelper htmlHelper)
        {
            var urlData = ControllerActionUrlParser.Parse(url);

            MvcHtmlString mvcHtml = ChildActionExtensions.Action(
                htmlHelper,
                urlData.ActionName,
                urlData.ControllerName,
                urlData.RouteValues);

            string html = mvcHtml.ToHtmlString();
            var hash = HashHelper.GetInt64(html).ToString();

            return new ReloadedTileRenderedHtmlData()
            {
                Url = url,
                Html = mvcHtml,
                Hash = hash
            };
        }

        public static MvcHtmlString PartialReloadedTile(this HtmlHelper htmlHelper, string url)
        {
            //htmlHelper
            //var viewContext = new ViewContext(controller.ControllerContext, new FakeView(), controller.ViewData, controller.TempData, TextWriter.Null);
            return htmlHelper.Action("Render", "ReloadedTiles", new { url });
        }

        private class FakeView : IView
        {
            public void Render(ViewContext viewContext, TextWriter writer)
            {
                throw new InvalidOperationException();
            }
        }
    }
}