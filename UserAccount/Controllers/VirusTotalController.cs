using System;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common.Helpers;
using Zidium.Core.ConfigDb;
using Zidium.UserAccount.Models.VirusTotal;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class VirusTotalController : SimpleCheckBaseController<EditSimpleModel>
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
            model.CommonWebsiteUrl = ConfigDbServicesHelper.GetUrlService().GetCommonWebsiteUrl();
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
            {
                model.CommonWebsiteUrl = ConfigDbServicesHelper.GetUrlService().GetCommonWebsiteUrl();
                return View(model);
            }

            model.SaveCommonSettings();
            model.SaveRule();
            CurrentAccountDbContext.SaveChanges();

            return RedirectToAction("ResultDetails", "UnitTests", new { id = model.Id });
        }

        [CanEditAllData]
        public ActionResult EditSimple(Guid? id = null, Guid? componentId = null)
        {
            var model = LoadSimpleCheck(id, componentId);
            model.CommonWebsiteUrl = ConfigDbServicesHelper.GetUrlService().GetCommonWebsiteUrl();
            if (id == null)
            {
                model.Period = TimeSpan.FromHours(6);
                model.ApiKey = GetVirusTotalApiKey();
            }
            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSimple(EditSimpleModel model)
        {
            if (!ModelState.IsValid)
            {
                model.CommonWebsiteUrl = ConfigDbServicesHelper.GetUrlService().GetCommonWebsiteUrl();
                return View(model);
            }

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
                .FirstOrDefault(t => t.TypeId == SystemUnitTestTypes.VirusTotalTestType.Id 
                                     && t.VirusTotalRule != null 
                                     && t.VirusTotalRule.Url == GetModelUrl(model) 
                                     && t.IsDeleted == false);
        }

        protected override string GetUnitTestDisplayName(EditSimpleModel model)
        {
            return "VirusTotal " + model.Url;
        }

        protected override void SetUnitTestParams(UnitTest unitTest, EditSimpleModel model)
        {
            if (unitTest.VirusTotalRule == null)
            {
                unitTest.VirusTotalRule = new UnitTestVirusTotalRule()
                {
                    NextStep = VirusTotalStep.Scan
                };
            }
            unitTest.VirusTotalRule.Url = GetModelUrl(model);
            SetVirusTotalApiKey(model.ApiKey);
        }

        protected string GetVirusTotalApiKey()
        {
            return CurrentAccountDbContext.GetAccountSettingService().VirusTotalApiKey;
        }

        protected void SetVirusTotalApiKey(string value)
        {
            CurrentAccountDbContext.GetAccountSettingService().VirusTotalApiKey = value;
        }

        protected override void SetModelParams(EditSimpleModel model, UnitTest unitTest)
        {
            model.Url = unitTest.VirusTotalRule.Url;
            model.ApiKey = GetVirusTotalApiKey();
        }

        protected override Guid GetUnitTestTypeId()
        {
            return SystemUnitTestTypes.VirusTotalTestType.Id;
        }

        protected string GetModelUrl(EditSimpleModel model)
        {
            return model.Url;
        }

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="userId"></param>
        public VirusTotalController(Guid accountId, Guid userId) : base(accountId, userId) { }

        public VirusTotalController() { }

        public string ComponentDisplayName(EditSimpleModel model)
        {
            return GetComponentDisplayName(model);
        }

        protected string GetHostFromUrl(string url)
        {
            var uri = new Uri(url);
            return uri.Host;
        }

        public override string GetComponentDisplayName(EditSimpleModel model)
        {
            string host = GetHostFromUrl(model.Url);
            return ComponentHelper.GetDisplayNameByHost(host);
        }

        protected override string GetComponentSystemName(EditSimpleModel model)
        {
            string host = GetHostFromUrl(model.Url);
            return ComponentHelper.GetSystemNameByHost(host);
        }

    }
}
