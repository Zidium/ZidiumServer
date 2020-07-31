using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models.ReloadedTileModels;

namespace Zidium.UserAccount.Controllers
{
    public class ReloadedTilesController : BaseController
    {
        public ActionResult Render(string url)
        {
            var model = new RenderModel()
            {
                Url = url
            };
            return PartialView("Render", model);
        }

        private HtmlHelper CreateHtmlHelper()
        {
            var viewContext = new ViewContext(
                    ControllerContext,
                    new FakeView(),
                    ViewData,
                    TempData,
                    TextWriter.Null);

            return new HtmlHelper(viewContext, new ViewPage());
        }

        
        [HttpPost]
        public ActionResult GetChanged(GetChangedModel model)
        {
            try
            {
                var htmlHelper = CreateHtmlHelper();

                // сгенерируем html всех плиток
                foreach (var tile in model.tiles)
                {
                    var newTile = ReloadedTileHelper.GetRenderdHtmlData(tile.url, htmlHelper);
                    if (newTile.Hash.Equals(tile.hash))
                    {
                        // содержимое НЕ изменилось, не нужно возвращать html
                        tile.html = null;
                        tile.hash = null;
                    }
                    else
                    {
                        // содержимое изменилось
                        tile.hash = newTile.Hash;
                        tile.html = newTile.Html.ToHtmlString();
                    }
                }
                model.tiles = model.tiles.Where(x => x.html != null).ToArray();
                return GetSuccessJsonResponse(model);
            }   
            catch(Exception ex)
            {
                return GetErrorJsonResponse(ex);
            }
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