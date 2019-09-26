using System;
using System.Data.SqlClient;
using System.Web.Mvc;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common.Helpers;
using Zidium.UserAccount.Models.SqlChecksModels;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class SqlChecksController : SimpleCheckBaseController<EditSimpleModel>
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

            var unitTest = SaveSimpleCheck(model);

            if (!Request.IsSmartBlocksRequest())
            {
                return RedirectToAction("ResultDetails", "UnitTests", new { id = unitTest.Id });
            }

            return GetSuccessJsonResponse(unitTest.Id);
        }

        protected string GetDbName(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            return builder.DataSource + "/" + builder.InitialCatalog;
        }

        protected override UnitTest FindSimpleCheck(EditSimpleModel model)
        {
            return null; // Всегда создаём новую
        }

        protected override string GetOldReplacementPart(UnitTest unitTest)
        {
            return GetDbName(unitTest.SqlRule.ConnectionString);
        }

        protected override string GetNewReplacementPart(EditSimpleModel model)
        {
            return GetDbName(model.ConnectionString);
        }

        protected override string GetUnitTestDisplayName(EditSimpleModel model)
        {
            return "Sql-запрос: " + model.Name;
        }

        protected override void SetUnitTestParams(UnitTest unitTest, EditSimpleModel model)
        {
            if (unitTest.SqlRule == null)
            {
                unitTest.SqlRule = new UnitTestSqlRule()
                {
                    Provider = Core.Common.DatabaseProviderType.MsSql,
                    OpenConnectionTimeoutMs = 3000,
                    CommandTimeoutMs = 30000
                };
            }
            unitTest.SqlRule.ConnectionString = model.ConnectionString;
            unitTest.SqlRule.Query = model.Query;
            unitTest.SqlRule.Provider = model.Provider;
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
            return SystemComponentTypes.DataBase.DisplayName;
        }

        protected override string GetTypeSystemName(EditSimpleModel model)
        {
            return SystemComponentTypes.DataBase.SystemName;
        }

        protected override void SetModelParams(EditSimpleModel model, UnitTest unitTest)
        {
            model.ConnectionString = unitTest.SqlRule.ConnectionString;
            model.Query = unitTest.SqlRule.Query;
            if (unitTest.DisplayName.StartsWith("Sql-запрос: "))
                model.Name = unitTest.DisplayName.Substring(12);
            model.Provider = unitTest.SqlRule.Provider;
        }

        protected override Guid GetUnitTestTypeId()
        {
            return SystemUnitTestTypes.SqlTestType.Id;
        }

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="userId"></param>
        public SqlChecksController(Guid accountId, Guid userId) : base(accountId, userId) { }

        public SqlChecksController() { }
        
        protected override string GetComponentSystemName(EditSimpleModel model)
        {
            return ComponentHelper.GetDynamicSystemName(new Guid());
        }
    }
}
