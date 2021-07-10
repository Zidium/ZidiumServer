using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models.ReloadedTileModels;

namespace Zidium.UserAccount.Controllers
{
    public class ReloadedTilesController : BaseController
    {
        public ReloadedTilesController(HtmlHelperGenerator htmlHelperGenerator, ILogger<ReloadedTilesController> logger) : base(logger)
        {
            _htmlHelperGenerator = htmlHelperGenerator;
        }

        private HtmlHelperGenerator _htmlHelperGenerator;

        public ActionResult Render(string url)
        {
            var model = new RenderModel()
            {
                Url = url
            };
            return PartialView("Render", model);
        }

        private IHtmlHelper CreateHtmlHelper()
        {
            return _htmlHelperGenerator.HtmlHelper(ViewData, TempData);
        }


        [HttpPost]
        public ActionResult GetChanged(GetChangedModel model)
        {
            try
            {
                if (model.tiles == null)
                    model.tiles = new GetChangedModelRow[0];

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
                        tile.html = htmlHelper.HtmlBlock(t => newTile.Html);
                    }
                }
                model.tiles = model.tiles.Where(x => x.html != null).ToArray();
                return GetSuccessJsonResponse(model);
            }
            catch (Exception ex)
            {
                return GetErrorJsonResponse(ex);
            }
        }

    }
}