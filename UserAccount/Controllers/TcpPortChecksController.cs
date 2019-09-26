using System;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common.Helpers;
using Zidium.UserAccount.Models.TcpPortChecksModels;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class TcpPortChecksController : SimpleCheckBaseController<EditSimpleModel>
    {
        public ActionResult Show(Guid id)
        {
            var model = new EditModel();
            model.Load(id, null);
            model.LoadRule();
            return View(model);
        }

        [CanEditAllData]
        public ActionResult Edit(Guid? id, Guid? componentId)
        {
            var model = new EditModel();
            model.Load(id, componentId);
            model.LoadRule();
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
            model.SaveRule();
            CurrentAccountDbContext.SaveChanges();

            return RedirectToAction("ResultDetails", "UnitTests", new { id = model.Id });
        }

        [CanEditAllData]
        public ActionResult EditSimple(Guid? id = null, Guid? componentId = null)
        {
            var model = LoadSimpleCheck(id, componentId);
            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSimple(EditSimpleModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var unitTest = SaveSimpleCheck(model);

            if (!Request.IsSmartBlocksRequest())
            {
                return RedirectToAction("ResultDetails", "UnitTests", new { id = unitTest.Id });
            }

            return GetSuccessJsonResponse(unitTest.Id);
        }

        protected override UnitTest FindSimpleCheck(EditSimpleModel model)
        {
            var unitTestRepository = CurrentAccountDbContext.GetUnitTestRepository();
            return unitTestRepository.QueryAll()
                .FirstOrDefault(t => t.TypeId == SystemUnitTestTypes.TcpPortTestType.Id 
                                     && t.TcpPortRule != null && t.TcpPortRule.Host == GetModelHost(model) && t.IsDeleted == false);
        }

        protected override string GetOldReplacementPart(UnitTest unitTest)
        {
            return unitTest.TcpPortRule.Host;
        }

        protected override string GetNewReplacementPart(EditSimpleModel model)
        {
            return GetModelHost(model);
        }

        protected override string GetUnitTestDisplayName(EditSimpleModel model)
        {
            return "Tcp порт " + GetModelHost(model) + ":" + model.Port;
        }

        protected override void SetUnitTestParams(UnitTest unitTest, EditSimpleModel model)
        {
            if (unitTest.TcpPortRule == null)
            {
                unitTest.TcpPortRule = new UnitTestTcpPortRule()
                {
                    Opened = true,
                };
            }
            unitTest.TcpPortRule.Host = GetModelHost(model);
            unitTest.TcpPortRule.Port = model.Port.Value;
            unitTest.TcpPortRule.Opened = model.Opened;
        }

        protected override void SetModelParams(EditSimpleModel model, UnitTest unitTest)
        {
            model.Host = unitTest.TcpPortRule.Host;
            model.Port = unitTest.TcpPortRule.Port;
            model.Opened = unitTest.TcpPortRule.Opened;
        }

        protected override Guid GetUnitTestTypeId()
        {
            return SystemUnitTestTypes.TcpPortTestType.Id;
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
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (UriFormatException)
            {
                return Json("Пожалуйста, укажите IP или домен", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CheckPort(string port)
        {
            int portInt = 0;
            if (Int32.TryParse(port, out portInt)==false)
            {
                return Json("Порт должен быть числом от 0 до 65535", JsonRequestBehavior.AllowGet);
            }

            if (portInt < 0 || portInt > 65535)
            {
                return Json("Порт должен быть числом от 0 до 65535", JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="userId"></param>
        public TcpPortChecksController(Guid accountId, Guid userId) : base(accountId, userId) { }

        public TcpPortChecksController() { }

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
    }
}
