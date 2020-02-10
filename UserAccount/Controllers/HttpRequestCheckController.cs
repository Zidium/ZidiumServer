using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.AccountsDb.Classes.UnitTests.HttpTests;
using Zidium.Core.Common.Helpers;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.Controls;
using Zidium.UserAccount.Models.HttpRequestCheckModels;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class HttpRequestCheckController : SimpleCheckBaseController<EditSimpleModel>
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

        protected KeyValueRowModel ConvertToKeyValueRow(HttpRequestUnitTestRuleData data)
        {
            return new KeyValueRowModel()
            {
                Id = data.Id.ToString(),
                Key = data.Key,
                Value = data.Value
            };
        }

        protected void AddRuleData(HttpRequestUnitTestRule rule, KeyValueRowModel row, HttpRequestUnitTestRuleDataType type)
        {
            var data = new HttpRequestUnitTestRuleData();
            data.Id = row.HasId ? new Guid(row.Id) : Guid.NewGuid();
            data.Key = row.Key;
            data.Value = row.Value;
            data.Type = type;
            data.Rule = rule;
            data.RuleId = rule.Id;
            rule.Datas.Add(data);
        }

        protected List<KeyValueRowModel> GetRuleDatas(HttpRequestUnitTestRule rule, HttpRequestUnitTestRuleDataType type)
        {
            var datas = rule.Datas.Where(x => x.Type == type).ToList();
            var rows = new List<KeyValueRowModel>();
            foreach (var data in datas)
            {
                var row = ConvertToKeyValueRow(data);
                rows.Add(row);
            }
            return rows;
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
            var unitTest = unitTestRepository.QueryAll()
                .FirstOrDefault(t => t.TypeId == SystemUnitTestTypes.HttpUnitTestType.Id && t.HttpRequestUnitTest != null && t.IsDeleted == false && t.HttpRequestUnitTest.Rules.Any(x => x.IsDeleted == false && x.SortNumber == 0 && x.Url == model.Url));
            return unitTest;
        }

        protected Func<HttpRequestUnitTestRule, bool> FindRulePredicate()
        {
            return x => x.IsDeleted == false && x.SortNumber == 0;
        }

        protected string GetHostFromUrl(string url)
        {
            var uri = new Uri(url);
            return uri.Host;
        }

        protected string GetComponentNameFromUrl(string url)
        {
            var uri = new Uri(url);
            return uri.Scheme + @"://" + uri.Authority;
        }

        protected override string GetUnitTestDisplayName(EditSimpleModel model)
        {
            return "Http-запрос для сайта " + model.Url;
        }

        protected override void SetUnitTestParams(UnitTest unitTest, EditSimpleModel model)
        {
            if (unitTest.HttpRequestUnitTest == null)
                unitTest.HttpRequestUnitTest = new HttpRequestUnitTest()
                {
                    UnitTest = unitTest
                };
            var rule = unitTest.HttpRequestUnitTest.Rules.FirstOrDefault(FindRulePredicate());
            if (rule == null)
            {
                rule = new HttpRequestUnitTestRule()
                {
                    Id = Guid.NewGuid(),
                    Method = HttpRequestMethod.Get,
                    ResponseCode = 200,
                    TimeoutSeconds = 5,
                    SortNumber = 0
                };
                unitTest.HttpRequestUnitTest.Rules.Add(rule);
            }
            rule.DisplayName = "Правило http-проверки сайта " + model.Url;
            rule.Url = model.Url;
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

        

        protected override void SetModelParams(EditSimpleModel model, UnitTest unitTest)
        {
            var rule = unitTest.HttpRequestUnitTest.Rules.First(FindRulePredicate());
            model.Url = rule.Url;
        }

        protected override Guid GetUnitTestTypeId()
        {
            return SystemUnitTestTypes.HttpUnitTestType.Id;
        }

        [CanEditAllData]
        public ActionResult DeleteRule(Guid id)
        {
            var repository = CurrentAccountDbContext.GetHttpRequestUnitTestRuleRepository();
            var rule = repository.GetById(id);
            CheckRuleDeletingPermissions(rule);
            var model = new DeleteConfirmationModel()
            {
                Id = id.ToString(),
                Title = "Удаление правила проверки",
                ModalMode = Request.IsAjaxRequest(),
                Message = "Вы действительно хотите удалить это правило?",
                ReturnUrl = Url.Action("Edit", new { id = rule.HttpRequestUnitTestId })
            };
            return View("~/Views/Shared/Dialogs/DeleteConfirmation.cshtml", model);
        }

        [CanEditAllData]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRule(DeleteConfirmationModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Shared/Dialogs/DeleteConfirmation.cshtml", model);
            var repository = CurrentAccountDbContext.GetHttpRequestUnitTestRuleRepository();
            var rule = repository.GetById(Guid.Parse(model.Id));
            CheckRuleDeletingPermissions(rule);
            var unitTestId = rule.HttpRequestUnitTestId;
            repository.Remove(rule);
            this.SetTempMessage(TempMessageType.Success, "Правило проверки удалёно");
            return RedirectToAction("Edit", new { id = unitTestId });
        }

        protected void CheckRuleDeletingPermissions(HttpRequestUnitTestRule rule)
        {
            if (rule.IsDeleted)
                throw new AlreadyDeletedException(rule.Id, Naming.HttpRequestUnitTestRule);
        }

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        public HttpRequestCheckController(Guid accountId, Guid userId) : base(accountId, userId) { }

        public HttpRequestCheckController() { }
    }
}