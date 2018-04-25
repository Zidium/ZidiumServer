using System;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.UserAccount.Models.DomainNamePaymentPeriodCheckModels;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class DomainNamePaymentPeriodChecksController : SimpleCheckBaseController<EditSimpleModel>
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

            model.Period = TimeSpan.FromDays(1);

            var unitTest = SaveSimpleCheck(model);

            return RedirectToAction("ResultDetails", "UnitTests", new { id = unitTest.Id });
        }

        protected override UnitTest FindSimpleCheck(EditSimpleModel model)
        {
            var unitTestRepository = CurrentAccountDbContext.GetUnitTestRepository();
            return unitTestRepository.QueryAll()
                .FirstOrDefault(t => t.TypeId == SystemUnitTestTypes.DomainNameTestType.Id && t.DomainNamePaymentPeriodRule != null && t.DomainNamePaymentPeriodRule.Domain == model.Domain && t.IsDeleted == false);
        }

        protected override string GetOldReplacementPart(UnitTest unitTest)
        {
            return unitTest.DomainNamePaymentPeriodRule.Domain;
        }

        protected override string GetNewReplacementPart(EditSimpleModel model)
        {
            return model.Domain;
        }

        protected override string GetUnitTestDisplayName(EditSimpleModel model)
        {
            return "Оплата домена " + model.Domain;
        }

        protected override void SetUnitTestParams(UnitTest unitTest, EditSimpleModel model)
        {
            if (unitTest.DomainNamePaymentPeriodRule == null)
            {
                unitTest.DomainNamePaymentPeriodRule = new UnitTestDomainNamePaymentPeriodRule()
                {
                    AlarmDaysCount = 14,
                    WarningDaysCount = 30
                };
            }
            unitTest.DomainNamePaymentPeriodRule.Domain = model.Domain;
        }

        protected override string GetComponentDisplayName(EditSimpleModel model)
        {
            return "Домен " + model.Domain;
        }

        protected override string GetFolderDisplayName(EditSimpleModel model)
        {
            return "Домены";
        }

        protected override string GetFolderSystemName(EditSimpleModel model)
        {
            return "Domains";
        }

        protected override string GetTypeDisplayName(EditSimpleModel model)
        {
            return SystemComponentTypes.Domain.DisplayName;
        }

        protected override string GetTypeSystemName(EditSimpleModel model)
        {
            return SystemComponentTypes.Domain.SystemName;
        }

        protected override void SetModelParams(EditSimpleModel model, UnitTest unitTest)
        {
            model.Domain = unitTest.DomainNamePaymentPeriodRule.Domain;
        }

        protected override Guid GetUnitTestTypeId()
        {
            return SystemUnitTestTypes.DomainNameTestType.Id;
        }


        // Для unit-тестов

        public DomainNamePaymentPeriodChecksController() { }

        public DomainNamePaymentPeriodChecksController(Guid accountId, Guid userId) : base(accountId, userId) { }

        public string ComponentDisplayName(EditSimpleModel model)
        {
            return GetComponentDisplayName(model);
        }

    }
}
