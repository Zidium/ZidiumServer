using System;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common.Helpers;
using Zidium.UserAccount.Models.SslCertificateExpirationDateChecksModels;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class SslCertificateExpirationDateChecksController : SimpleCheckBaseController<EditSimpleModel>
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
                .FirstOrDefault(t => t.TypeId == SystemUnitTestTypes.SslTestType.Id && t.SslCertificateExpirationDateRule != null && t.SslCertificateExpirationDateRule.Url == model.Url && t.IsDeleted == false);
        }

        protected override string GetOldReplacementPart(UnitTest unitTest)
        {
            return unitTest.SslCertificateExpirationDateRule.Url;
        }

        protected override string GetNewReplacementPart(EditSimpleModel model)
        {
            return model.Url;
        }

        protected override string GetUnitTestDisplayName(EditSimpleModel model)
        {
            return "Срок годности SSL-сертификата сайта " + model.Url;
        }

        protected override void SetUnitTestParams(UnitTest unitTest, EditSimpleModel model)
        {
            if (unitTest.SslCertificateExpirationDateRule == null)
            {
                unitTest.SslCertificateExpirationDateRule = new UnitTestSslCertificateExpirationDateRule()
                {
                    WarningDaysCount = 30,
                    AlarmDaysCount = 14
                };
            }
            unitTest.SslCertificateExpirationDateRule.Url = model.Url;
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

        protected override void SetModelParams(EditSimpleModel model, UnitTest unitTest)
        {
            model.Url = unitTest.SslCertificateExpirationDateRule.Url;
        }

        protected override Guid GetUnitTestTypeId()
        {
            return SystemUnitTestTypes.SslTestType.Id;
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
