using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zidium.UserAccount.Models.Examples.Tiles;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class TilesExamplesController : BaseController
    {
        private static Random _random = new Random();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Tiles()
        {
            return View();
        }

        public ActionResult ReloadedTiles()
        {
            return View();
        }

        public ActionResult CounterValue(string id)
        {
            // счетчик меняет своё значение раз в 10 раз в среднем
            var model = new ReloadedSampleModel()
            {
                Id = id,
                Value = 0
            };
            int value = _random.Next(100);
            if (value > 90)
            {
                model.Value = value;
            }            
            return PartialView(model);
        }
    }
}