using System;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class UnitTestTypesController : ContextController
    {
        public ActionResult Index(string search = null)
        {
            var repository = CurrentAccountDbContext.GetUnitTestTypeRepository();
            var items = repository
                .QueryAllForGui(search)
                .OrderBy(t => t.IsSystem)
                .ThenBy(t => t.DisplayName);

            var model = new UnitTestTypeListModel()
            {
                Search = search,
                Items = items
            };
            return View(model);
        }

        public ActionResult Show(Guid id)
        {
            var repository = CurrentAccountDbContext.GetUnitTestTypeRepository();
            var unitTestType = repository.GetById(id);

            var model = new UnitTestTypeShowModel()
            {
                Id = unitTestType.Id,
                DisplayName = unitTestType.DisplayName,
                SystemName = unitTestType.SystemName,
                ActualTime = TimeSpanHelper.FromSeconds(unitTestType.ActualTimeSecs),
                ActualTimeDefault = UnitTestHelper.GetDefaultActualTime(),
                NoSignalColor = unitTestType.NoSignalColor,
                NoSignalColorDefault = ObjectColor.Red,
                IsDeleted = unitTestType.IsDeleted,
                IsSystem = unitTestType.IsSystem
            };
            return View(model);
        }

        [CanEditAllData]
        public ActionResult Add()
        {
            var model = new UnitTestTypeEditModel();
            model.InitReadOnlyData();
            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(UnitTestTypeEditModel model)
        {
            model.InitReadOnlyData();

            if (!ModelState.IsValid)
                return View(model);

            var client = GetDispatcherClient();
            var response = client.GetOrCreateUnitTestType(CurrentUser.AccountId, new GetOrCreateUnitTestTypeRequestData()
            {
                SystemName = model.SystemName,
                DisplayName = model.DisplayName,
                ActualTimeSecs = TimeSpanHelper.GetSeconds(model.ActualTime),
                NoSignalColor = model.NoSignalColor?.GetSelectedOne()
            });

            response.Check();

            var unitTestType = response.Data;

            this.SetTempMessage(TempMessageType.Success, string.Format("Добавлен тип юнит-теста <a href='{1}' class='alert-link'>{0}</a>", unitTestType.DisplayName, Url.Action("Show", new { id = unitTestType.Id })));
            return RedirectToAction("Index");
        }

        [CanEditAllData]
        public ActionResult Edit(Guid id)
        {
            var repository = CurrentAccountDbContext.GetUnitTestTypeRepository();
            var unitTestType = repository.GetById(id);
            CheckEditingPermissions(unitTestType);

            var model = new UnitTestTypeEditModel()
            {
                Id = unitTestType.Id,
                DisplayName = unitTestType.DisplayName,
                SystemName = unitTestType.SystemName,
                ActualTime = TimeSpanHelper.FromSeconds(unitTestType.ActualTimeSecs),
                NoSignalColor = ColorStatusSelectorValue.FromColor(unitTestType.NoSignalColor),
                IsDeleted = unitTestType.IsDeleted,
                IsSystem = unitTestType.IsSystem
            };
            model.InitReadOnlyData();

            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UnitTestTypeEditModel model)
        {
            model.InitReadOnlyData();

            if (!ModelState.IsValid)
                return View(model);

            var repository = CurrentAccountDbContext.GetUnitTestTypeRepository();
            var unitTestType = repository.GetById(model.Id);
            CheckEditingPermissions(unitTestType);

            var client = GetDispatcherClient();
            var response = client.UpdateUnitTestType(CurrentUser.AccountId, new UpdateUnitTestTypeRequestData()
            {
                UnitTestTypeId = unitTestType.Id,
                DisplayName = model.DisplayName,
                SystemName = model.SystemName,
                ActualTimeSecs = TimeSpanHelper.GetSeconds(model.ActualTime),
                NoSignalColor = model.NoSignalColor.GetSelectedOne()
            });

            response.Check();

            return RedirectToAction("Show", new { id = model.Id });
        }

        protected void CheckEditingPermissions(UnitTestType unitTestType)
        {
            if (unitTestType.IsSystem)
                throw new CantEditSystemObjectException(unitTestType.Id, Naming.UnitTestType);
        }

        [CanEditAllData]
        public ActionResult Delete(Guid id)
        {
            var repository = CurrentAccountDbContext.GetUnitTestTypeRepository();
            var unitTestType = repository.GetById(id);
            CheckDeletingPermissions(unitTestType);
            var model = new DeleteConfirmationModel()
            {
                Id = id.ToString(),
                Title = "Удаление типа юнит-теста",
                ModalMode = Request.IsAjaxRequest(),
                Message = "Вы действительно хотите удалить этот тип проверки?",
                ReturnUrl = Url.Action("Index")
            };
            return View("~/Views/Shared/Dialogs/DeleteConfirmation.cshtml", model);
        }

        [CanEditAllData]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(DeleteConfirmationModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Shared/Dialogs/DeleteConfirmation.cshtml", model);

            var repository = CurrentAccountDbContext.GetUnitTestTypeRepository();
            var unitTestType = repository.GetById(Guid.Parse(model.Id));
            CheckDeletingPermissions(unitTestType);

            var client = GetDispatcherClient();
            var response = client.DeleteUnitTestType(CurrentUser.AccountId, new DeleteUnitTestTypeRequestData()
            {
                UnitTestTypeId = unitTestType.Id
            });
            if (!response.Success)
                throw new UserFriendlyException(response.ErrorMessage);

            this.SetTempMessage(TempMessageType.Success, "Тип проверки удалён");
            return RedirectToAction("Index");
        }

        protected void CheckDeletingPermissions(UnitTestType unitTestType)
        {
            if (unitTestType.IsSystem)
                throw new CantDeleteSystemObjectException(unitTestType.Id, Naming.UnitTestType);
            if (unitTestType.IsDeleted)
                throw new AlreadyDeletedException(unitTestType.Id, Naming.UnitTestType);
        }

        public JsonResult CheckSystemName(UnitTestTypeEditModel model)
        {
            var repository = CurrentAccountDbContext.GetUnitTestTypeRepository();
            var unitTestType = repository.GetOneOrNullBySystemName(model.SystemName);
            if (unitTestType != null && (model.Id == Guid.Empty || model.Id != unitTestType.Id))
                return Json("Тип проверки с таким системным именем уже существует", JsonRequestBehavior.AllowGet);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="userId"></param>
        public UnitTestTypesController(Guid accountId, Guid userId) : base(accountId, userId) { }

        public UnitTestTypesController() { }
    }
}