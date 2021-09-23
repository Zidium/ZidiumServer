using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zidium.Common;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.HttpRequestCheckModels;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class HttpRequestCheckController : SimpleCheckBaseController<EditSimpleModel>
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

        protected Func<HttpRequestUnitTestRuleForRead, bool> FindRulePredicate()
        {
            return x => x.SortNumber == 0;
        }

        protected string GetHostFromUrl(string url)
        {
            var uri = new Uri(url);
            return uri.Host;
        }

        protected override string GetUnitTestDisplayName(EditSimpleModel model)
        {
            return "Http-запрос для сайта " + model.Url;
        }

        protected override void SetUnitTestParams(Guid unitTestId, EditSimpleModel model, IStorage storage)
        {
            using (var transaction = storage.BeginTransaction())
            {
                var httpRequestUnitTest = GetStorage().HttpRequestUnitTests.GetOneOrNullByUnitTestId(unitTestId);
                if (httpRequestUnitTest == null)
                {
                    var httpRequestUnitTestForAdd = new HttpRequestUnitTestForAdd()
                    {
                        UnitTestId = unitTestId
                    };
                    GetStorage().HttpRequestUnitTests.Add(httpRequestUnitTestForAdd);
                }

                var rule = GetStorage().HttpRequestUnitTestRules.GetByUnitTestId(unitTestId).FirstOrDefault(FindRulePredicate());
                if (rule == null)
                {
                    var ruleForAdd = new HttpRequestUnitTestRuleForAdd()
                    {
                        Id = Ulid.NewUlid(),
                        HttpRequestUnitTestId = unitTestId,
                        Method = HttpRequestMethod.Get,
                        ResponseCode = 200,
                        TimeoutSeconds = 5,
                        SortNumber = 0,
                        DisplayName = string.Empty,
                        Url = string.Empty
                    };
                    GetStorage().HttpRequestUnitTestRules.Add(ruleForAdd);
                    rule = GetStorage().HttpRequestUnitTestRules.GetOneById(ruleForAdd.Id);
                }

                var ruleForUpdate = rule.GetForUpdate();
                ruleForUpdate.DisplayName.Set("Правило http-проверки сайта " + model.Url);
                ruleForUpdate.Url.Set(model.Url);
                GetStorage().HttpRequestUnitTestRules.Update(ruleForUpdate);

                transaction.Commit();
            }
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

        protected override void SetModelParams(EditSimpleModel model, UnitTestForRead unitTest, IStorage storage)
        {
            var rule = GetStorage().HttpRequestUnitTestRules.GetByUnitTestId(unitTest.Id).First(FindRulePredicate());
            model.Url = rule.Url;
        }

        protected override Guid GetUnitTestTypeId()
        {
            return SystemUnitTestType.HttpUnitTestType.Id;
        }

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        internal HttpRequestCheckController(Guid userId) : base(userId) { }

        public HttpRequestCheckController(ILogger<HttpRequestCheckController> logger) : base(logger)
        {
        }
    }
}