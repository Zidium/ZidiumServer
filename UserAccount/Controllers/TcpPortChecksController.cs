using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zidium.Common;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.TcpPortChecksModels;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class TcpPortChecksController : SimpleCheckBaseController<EditSimpleModel>
    {
        public ActionResult Show(Guid id)
        {
            var model = new EditModel();
            model.Load(id, null, GetStorage());
            model.LoadRule(id, GetStorage());
            model.UnitTestBreadCrumbs = UnitTestBreadCrumbsModel.Create(id, GetStorage());
            return View(model);
        }

        [CanEditAllData]
        public ActionResult Edit(Guid? id, Guid? componentId)
        {
            var model = new EditModel();
            model.Load(id, componentId, GetStorage());
            model.LoadRule(id, GetStorage());
            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditModel model)
        {
            try
            {
                model.Validate();
            }
            catch (UserFriendlyException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
            }

            if (!ModelState.IsValid)
                return View(model);

            model.SaveCommonSettings();
            model.SaveRule(GetStorage());

            return RedirectToAction("ResultDetails", "UnitTests", new { id = model.Id });
        }

        [CanEditAllData]
        public ActionResult EditSimple(Guid? id = null, Guid? componentId = null)
        {
            var model = LoadSimpleCheck(id, componentId, GetStorage());
            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSimple(EditSimpleModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var unitTestId = SaveSimpleCheck(model, GetStorage());

            if (!Request.IsSmartBlocksRequest())
            {
                return RedirectToAction("ResultDetails", "UnitTests", new { id = unitTestId });
            }

            return GetSuccessJsonResponse(unitTestId);
        }

        protected override string GetUnitTestDisplayName(EditSimpleModel model)
        {
            return "Tcp порт " + GetModelHost(model) + ":" + model.Port;
        }

        protected override void SetUnitTestParams(Guid unitTestId, EditSimpleModel model, IStorage storage)
        {
            using (var transaction = storage.BeginTransaction())
            {
                var rule = storage.UnitTestTcpPortRules.GetOneOrNullByUnitTestId(unitTestId);
                if (rule == null)
                {
                    var ruleForAdd = new UnitTestTcpPortRuleForAdd()
                    {
                        UnitTestId = unitTestId
                    };
                    storage.UnitTestTcpPortRules.Add(ruleForAdd);
                    rule = storage.UnitTestTcpPortRules.GetOneOrNullByUnitTestId(unitTestId);
                }

                var ruleForUpdate = rule.GetForUpdate();
                ruleForUpdate.Host.Set(GetModelHost(model));
                ruleForUpdate.Port.Set(model.Port.Value);
                ruleForUpdate.Opened.Set(model.Opened);
                storage.UnitTestTcpPortRules.Update(ruleForUpdate);

                transaction.Commit();
            }
        }

        protected override void SetModelParams(EditSimpleModel model, UnitTestForRead unitTest, IStorage storage)
        {
            var rule = storage.UnitTestTcpPortRules.GetOneOrNullByUnitTestId(unitTest.Id);
            model.Host = rule.Host;
            model.Port = rule.Port;
            model.Opened = rule.Opened;
        }

        protected override Guid GetUnitTestTypeId()
        {
            return SystemUnitTestType.TcpPortTestType.Id;
        }

        protected string GetModelHost(EditSimpleModel model)
        {
            if (Uri.CheckHostName(model.Host) == UriHostNameType.Unknown)
                throw new UriFormatException();

            return model.Host;
        }

        public JsonResult CheckHost(string host)
        {
            try
            {
                var result = GetModelHost(new EditSimpleModel() { Host = host });
                return Json(true);
            }
            catch (UriFormatException)
            {
                return Json("Пожалуйста, укажите IP или домен");
            }
        }

        public JsonResult CheckPort(string port)
        {
            int portInt = 0;
            if (Int32.TryParse(port, out portInt) == false)
            {
                return Json("Порт должен быть числом от 0 до 65535");
            }

            if (portInt < 0 || portInt > 65535)
            {
                return Json("Порт должен быть числом от 0 до 65535");
            }
            return Json(true);
        }

        public string ComponentDisplayName(EditSimpleModel model)
        {
            return GetComponentDisplayName(model);
        }

        protected override string GetComponentSystemName(EditSimpleModel model)
        {
            return ComponentHelper.GetSystemNameByHost(model.Host);
        }

        public override string GetComponentDisplayName(EditSimpleModel model)
        {
            return ComponentHelper.GetDisplayNameByHost(model.Host);
        }

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="userId"></param>
        internal TcpPortChecksController(Guid userId) : base(userId) { }

        public TcpPortChecksController(ILogger<TcpPortChecksController> logger) : base(logger)
        {
        }
    }
}
