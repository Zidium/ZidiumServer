using System;
using System.Threading;
using System.Web.Mvc;
using Zidium.UserAccount.Models.Examples.Smart;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class SmartExamplesController : BaseController
    {
        public ActionResult Example1()
        {
            return View();
        }

        public ActionResult Example1Partial()
        {
            var model = new Example1PartialModel();
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Example1Partial(Example1PartialModel model)
        {
            try
            {
                Thread.Sleep(3000);

                if (model.Name == "1")
                    ModelState.AddModelError("Name", "Name can't be 1");

                if (model.Name == "2")
                    throw new Exception("Name can't be 2");

                if (!ModelState.IsValid)
                    return PartialView(model);

                return GetSuccessJsonResponse(model.Name);
            }
            catch (Exception exception)
            {
                return GetErrorJsonResponse(exception);
            }
        }

        public ActionResult Example2()
        {
            var model = new Example2FiltersModel();
            return View(model);
        }

        public ActionResult Example2Partial(Example2FiltersModel filters)
        {
            Thread.Sleep(3000);

            if (filters.Name == "2")
                throw new Exception("Name can't be 2");

            var model = new Example2PartialModel()
            {
                Value = Guid.NewGuid()
            };

            return PartialView(model);
        }

        public ActionResult Example3()
        {
            var model = new Example2FiltersModel();
            return View(model);
        }

        public ActionResult Example4()
        {
            return View();
        }

        public JsonResult Example4Action(string name)
        {
            Thread.Sleep(3000);

            if (name == "2")
                throw new Exception("Name can't be 2");

            return GetSuccessJsonResponse(name);
        }
    }
}