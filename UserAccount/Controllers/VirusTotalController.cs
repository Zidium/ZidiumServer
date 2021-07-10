using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zidium.Common;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.VirusTotal;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class VirusTotalController : SimpleCheckBaseController<EditSimpleModel>
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
            {
                return View(model);
            }

            model.SaveCommonSettings();
            model.SaveRule(GetStorage());

            return RedirectToAction("ResultDetails", "UnitTests", new { id = model.Id });
        }

        [CanEditAllData]
        public ActionResult EditSimple(Guid? id = null, Guid? componentId = null)
        {
            var model = LoadSimpleCheck(id, componentId, GetStorage());
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
                return View(model);
            }

            Uri uri;
            try
            {
                uri = new Uri(model.Url);
            }
            catch (UriFormatException)
            {
                try
                {
                    uri = new Uri("http://" + model.Url);
                }
                catch
                {
                    ModelState.AddModelError("Url", "Пожалуйста, укажите корректный Url");
                    return View(model);
                }
            }
            model.Url = uri.OriginalString;

            var unitTestId = SaveSimpleCheck(model, GetStorage());

            if (!Request.IsSmartBlocksRequest())
            {
                return RedirectToAction("ResultDetails", "UnitTests", new { id = unitTestId });
            }

            return GetSuccessJsonResponse(unitTestId);
        }

        protected override string GetUnitTestDisplayName(EditSimpleModel model)
        {
            return "VirusTotal " + model.Url;
        }

        protected override void SetUnitTestParams(Guid unitTestId, EditSimpleModel model, IStorage storage)
        {
            using (var transaction = storage.BeginTransaction())
            {
                var rule = storage.UnitTestVirusTotalRules.GetOneOrNullByUnitTestId(unitTestId);
                if (rule == null)
                {
                    var ruleForAdd = new UnitTestVirusTotalRuleForAdd()
                    {
                        UnitTestId = unitTestId,
                        NextStep = VirusTotalStep.Scan,
                        Url = string.Empty
                    };
                    storage.UnitTestVirusTotalRules.Add(ruleForAdd);
                    rule = storage.UnitTestVirusTotalRules.GetOneOrNullByUnitTestId(unitTestId);
                }

                var ruleForUpdate = rule.GetForUpdate();
                ruleForUpdate.Url.Set(GetModelUrl(model));
                SetVirusTotalApiKey(model.ApiKey);
                storage.UnitTestVirusTotalRules.Update(ruleForUpdate);

                transaction.Commit();
            }
        }

        protected string GetVirusTotalApiKey()
        {
            var service = new AccountSettingService(GetStorage());
            return service.VirusTotalApiKey;
        }

        protected void SetVirusTotalApiKey(string value)
        {
            var service = new AccountSettingService(GetStorage());
            service.VirusTotalApiKey = value;
        }

        protected override void SetModelParams(EditSimpleModel model, UnitTestForRead unitTest, IStorage storage)
        {
            var rule = storage.UnitTestVirusTotalRules.GetOneOrNullByUnitTestId(unitTest.Id);
            model.Url = rule.Url;
            model.ApiKey = GetVirusTotalApiKey();
        }

        protected override Guid GetUnitTestTypeId()
        {
            return SystemUnitTestType.VirusTotalTestType.Id;
        }

        protected string GetModelUrl(EditSimpleModel model)
        {
            return model.Url;
        }

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

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="userId"></param>
        internal VirusTotalController(Guid userId) : base(userId) { }

        public VirusTotalController() { }

    }
}
