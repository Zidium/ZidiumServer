using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Zidium.Common;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.SqlChecksModels;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class SqlChecksController : SimpleCheckBaseController<EditSimpleModel>
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

            // Проверим, что строка соединения валидная
            try
            {
                var temp = new SqlConnectionStringBuilder(model.ConnectionString);
            }
            catch (Exception exception)
            {
                ModelState.AddModelError("ConnectionString", exception.Message);
                return View(model);
            }

            var unitTestId = SaveSimpleCheck(model, GetStorage());

            if (!Request.IsSmartBlocksRequest())
            {
                return RedirectToAction("ResultDetails", "UnitTests", new { id = unitTestId });
            }

            return GetSuccessJsonResponse(unitTestId);
        }

        protected string GetDbName(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            return builder.DataSource + "/" + builder.InitialCatalog;
        }

        protected override string GetUnitTestDisplayName(EditSimpleModel model)
        {
            return "Sql-запрос: " + model.Name;
        }

        protected override void SetUnitTestParams(Guid unitTestId, EditSimpleModel model, IStorage storage)
        {
            using (var transaction = storage.BeginTransaction())
            {
                var rule = storage.UnitTestSqlRules.GetOneOrNullByUnitTestId(unitTestId);
                if (rule == null)
                {
                    var ruleForAdd = new UnitTestSqlRuleForAdd()
                    {
                        UnitTestId = unitTestId,
                        Provider = SqlRuleDatabaseProviderType.MsSql,
                        OpenConnectionTimeoutMs = 3000,
                        CommandTimeoutMs = 30000
                    };
                    storage.UnitTestSqlRules.Add(ruleForAdd);
                    rule = storage.UnitTestSqlRules.GetOneOrNullByUnitTestId(unitTestId);
                }

                var ruleForUpdate = rule.GetForUpdate();
                ruleForUpdate.ConnectionString.Set(model.ConnectionString);
                ruleForUpdate.Query.Set(model.Query);
                ruleForUpdate.Provider.Set(model.Provider);
                storage.UnitTestSqlRules.Update(ruleForUpdate);

                transaction.Commit();
            }
        }

        public override string GetComponentDisplayName(EditSimpleModel model)
        {
            return "База данных " + GetDbName(model.ConnectionString);
        }

        protected override string GetFolderDisplayName(EditSimpleModel model)
        {
            return "Базы данных";
        }

        protected override string GetFolderSystemName(EditSimpleModel model)
        {
            return "Databases";
        }

        protected override string GetTypeDisplayName(EditSimpleModel model)
        {
            return SystemComponentType.Database.DisplayName;
        }

        protected override string GetTypeSystemName(EditSimpleModel model)
        {
            return SystemComponentType.Database.SystemName;
        }

        protected override void SetModelParams(EditSimpleModel model, UnitTestForRead unitTest, IStorage storage)
        {
            var rule = storage.UnitTestSqlRules.GetOneOrNullByUnitTestId(unitTest.Id);
            model.ConnectionString = rule.ConnectionString;
            model.Query = rule.Query;
            if (unitTest.DisplayName.StartsWith("Sql-запрос: "))
                model.Name = unitTest.DisplayName.Substring(12);
            model.Provider = rule.Provider;
        }

        protected override Guid GetUnitTestTypeId()
        {
            return SystemUnitTestType.SqlTestType.Id;
        }

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="userId"></param>
        internal SqlChecksController(Guid userId) : base(userId) { }

        public SqlChecksController(ILogger<SqlChecksController> logger) : base(logger)
        {
        }

        protected override string GetComponentSystemName(EditSimpleModel model)
        {
            return ComponentHelper.GetDynamicSystemName(new Guid());
        }
    }
}
