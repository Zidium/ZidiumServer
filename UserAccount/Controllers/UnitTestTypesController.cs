using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.Core.Api;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class UnitTestTypesController : BaseController
    {
        public ActionResult Index(string search = null)
        {
            var items = GetStorage().UnitTestTypes.Filter(search, 100);

            var model = new UnitTestTypeListModel()
            {
                Search = search,
                Items = items
            };
            return View(model);
        }

        public ActionResult Show(Guid id)
        {
            var unitTestType = GetStorage().UnitTestTypes.GetOneById(id);

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
            var response = client.GetOrCreateUnitTestType(new GetOrCreateUnitTestTypeRequestDataDto()
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
            var unitTestType = GetStorage().UnitTestTypes.GetOneById(id);
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

            var unitTestType = GetStorage().UnitTestTypes.GetOneById(model.Id);
            CheckEditingPermissions(unitTestType);

            var client = GetDispatcherClient();
            var response = client.UpdateUnitTestType(new UpdateUnitTestTypeRequestData()
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

        protected void CheckEditingPermissions(UnitTestTypeForRead unitTestType)
        {
            if (unitTestType.IsSystem)
                throw new UserFriendlyException("Нельзя изменять системный тип проверки");
        }

        [CanEditAllData]
        public ActionResult Delete(Guid id)
        {
            var unitTestType = GetStorage().UnitTestTypes.GetOneById(id);
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

            var unitTestType = GetStorage().UnitTestTypes.GetOneById(Guid.Parse(model.Id));
            CheckDeletingPermissions(unitTestType);

            var client = GetDispatcherClient();
            var response = client.DeleteUnitTestType(new DeleteUnitTestTypeRequestData()
            {
                UnitTestTypeId = unitTestType.Id
            });
            if (!response.Success)
                throw new UserFriendlyException(response.ErrorMessage);

            this.SetTempMessage(TempMessageType.Success, "Тип проверки удалён");
            return RedirectToAction("Index");
        }

        protected void CheckDeletingPermissions(UnitTestTypeForRead unitTestType)
        {
            if (unitTestType.IsSystem)
                throw new UserFriendlyException("Незлья удалять системный тип проверки");
            if (unitTestType.IsDeleted)
                throw new UserFriendlyException("Тип проверки уже удалён");
        }

        public JsonResult CheckSystemName(UnitTestTypeEditModel model)
        {
            var unitTestType = GetStorage().UnitTestTypes.GetOneOrNullBySystemName(model.SystemName);
            if (unitTestType != null && (model.Id == Guid.Empty || model.Id != unitTestType.Id))
                return Json("Тип проверки с таким системным именем уже существует");
            return Json(true);
        }

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="userId"></param>
        internal UnitTestTypesController(Guid userId) : base(userId) { }

        public UnitTestTypesController() { }
    }
}