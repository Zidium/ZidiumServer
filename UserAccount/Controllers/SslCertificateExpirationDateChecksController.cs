using System;
using System.Linq;
using System.Web.Mvc;
using Zidium.Common;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.SslCertificateExpirationDateChecksModels;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class SslCertificateExpirationDateChecksController : SimpleCheckBaseController<EditSimpleModel>
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
        public ActionResult Edit(EditModel model)
        {
            model.Period = TimeSpan.FromDays(1);

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
            model.Period = TimeSpan.FromDays(1);

            var unitTestId = SaveSimpleCheck(model, GetStorage());

            if (!Request.IsSmartBlocksRequest())
            {
                return RedirectToAction("ResultDetails", "UnitTests", new { id = unitTestId });
            }

            return GetSuccessJsonResponse(unitTestId);
        }

        protected override string GetUnitTestDisplayName(EditSimpleModel model)
        {
            return "Срок годности SSL-сертификата сайта " + model.Url;
        }

        protected override void SetUnitTestParams(Guid unitTestId, EditSimpleModel model, IStorage storage)
        {
            using (var transaction = storage.BeginTransaction())
            {
                var rule = storage.UnitTestSslCertificateExpirationDateRules.GetOneOrNullByUnitTestId(unitTestId);
                if (rule == null)
                {
                    var ruleForAdd = new UnitTestSslCertificateExpirationDateRuleForAdd()
                    {
                        UnitTestId = unitTestId,
                        WarningDaysCount = 30,
                        AlarmDaysCount = 14
                    };
                    storage.UnitTestSslCertificateExpirationDateRules.Add(ruleForAdd);
                    rule = storage.UnitTestSslCertificateExpirationDateRules.GetOneOrNullByUnitTestId(unitTestId);
                }

                var ruleForUpdate = rule.GetForUpdate();
                ruleForUpdate.Url.Set(model.Url);
                storage.UnitTestSslCertificateExpirationDateRules.Update(ruleForUpdate);

                transaction.Commit();
            }
        }

        protected string GetHostFromUrl(string url)
        {
            var uri = new Uri(url);
            return uri.Host;
        }

        public override string GetComponentDisplayName(EditSimpleModel model)
        {
            var host = GetHostFromUrl(model.Url);
            return ComponentHelper.GetDisplayNameByHost(host);
        }

        protected override string GetComponentSystemName(EditSimpleModel model)
        {
            var host = GetHostFromUrl(model.Url);
            return ComponentHelper.GetSystemNameByHost(host);
        }

        protected override void SetModelParams(EditSimpleModel model, UnitTestForRead unitTest, IStorage storage)
        {
            var rule = storage.UnitTestSslCertificateExpirationDateRules.GetOneOrNullByUnitTestId(unitTest.Id);
            model.Url = rule.Url;
        }

        protected override Guid GetUnitTestTypeId()
        {
            return SystemUnitTestType.SslTestType.Id;
        }

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="userId"></param>
        public SslCertificateExpirationDateChecksController(Guid accountId, Guid userId) : base(accountId, userId) { }

        public SslCertificateExpirationDateChecksController() { }
    }
}
