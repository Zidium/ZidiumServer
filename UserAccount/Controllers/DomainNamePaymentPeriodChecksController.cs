using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zidium.Common;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.DomainNamePaymentPeriodCheckModels;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class DomainNamePaymentPeriodChecksController : SimpleCheckBaseController<EditSimpleModel>
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
            model.SaveRule(model.Id.Value, GetStorage());

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
            return "Оплата домена " + model.Domain;
        }

        protected override void SetUnitTestParams(Guid unitTestId, EditSimpleModel model, IStorage storage)
        {
            using (var transaction = storage.BeginTransaction())
            {
                var rule = storage.DomainNamePaymentPeriodRules.GetOneOrNullByUnitTestId(unitTestId);
                if (rule == null)
                {
                    var newRule = new UnitTestDomainNamePaymentPeriodRuleForAdd()
                    {
                        UnitTestId = unitTestId,
                        AlarmDaysCount = 14,
                        WarningDaysCount = 30
                    };
                    storage.DomainNamePaymentPeriodRules.Add(newRule);
                }

                var ruleForUpdate = new UnitTestDomainNamePaymentPeriodRuleForUpdate(unitTestId);
                ruleForUpdate.Domain.Set(model.Domain);
                storage.DomainNamePaymentPeriodRules.Update(ruleForUpdate);

                transaction.Commit();
            }
        }

        protected override void SetModelParams(EditSimpleModel model, UnitTestForRead unitTest, IStorage storage)
        {
            var rule = storage.DomainNamePaymentPeriodRules.GetOneByUnitTestId(unitTest.Id);
            model.Domain = rule.Domain;
        }

        protected override Guid GetUnitTestTypeId()
        {
            return SystemUnitTestType.DomainNameTestType.Id;
        }

        // Для unit-тестов

        internal DomainNamePaymentPeriodChecksController(Guid userId) : base(userId) { }

        public DomainNamePaymentPeriodChecksController(ILogger<DomainNamePaymentPeriodChecksController> logger) : base(logger)
        {
        }

        protected override string GetComponentSystemName(EditSimpleModel model)
        {
            return ComponentHelper.GetSystemNameByHost(model.Domain);
        }

        public override string GetComponentDisplayName(EditSimpleModel model)
        {
            return ComponentHelper.GetDisplayNameByHost(model.Domain);
        }
    }
}
