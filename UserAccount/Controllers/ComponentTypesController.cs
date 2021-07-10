using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.Storage;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class ComponentTypesController : BaseController
    {
        public ActionResult Index(string search = null)
        {
            var data = GetStorage().ComponentTypes.Filter(search, 100);

            var model = new ComponentTypesListModel
            {
                Search = search,
                ComponentTypes = data
            };
            return View(model);
        }

        public ActionResult Show(Guid id)
        {
            var componentType = GetStorage().ComponentTypes.GetOneById(id);
            var model = new ComponentTypeEditModel()
            {
                Id = componentType.Id,
                DisplayName = componentType.DisplayName,
                SystemName = componentType.SystemName,
                IsCommon = componentType.IsSystem,
                IsDeleted = componentType.IsDeleted
            };
            return View(model);
        }

        [CanEditAllData]
        public ActionResult Add()
        {
            var model = new ComponentTypeEditModel();
            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(ComponentTypeEditModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = GetDispatcherClient();
            var response = client.GetOrCreateComponentType(new GetOrCreateComponentTypeRequestDataDto()
            {
                SystemName = model.SystemName,
                DisplayName = model.DisplayName
            });

            if (!response.Success)
                throw new UserFriendlyException(response.ErrorMessage);

            var componentType = response.Data;

            this.SetTempMessage(TempMessageType.Success, string.Format("Добавлен тип компонента <a href='{1}' class='alert-link'>{0}</a>", componentType.DisplayName, Url.Action("Show", new { id = componentType.Id })));
            return RedirectToAction("Index");
        }

        [CanEditAllData]
        public ActionResult Edit(Guid id)
        {
            var componentType = GetStorage().ComponentTypes.GetOneById(id);
            CheckEditingPermissions(componentType);
            var model = new ComponentTypeEditModel()
            {
                Id = componentType.Id,
                DisplayName = componentType.DisplayName,
                SystemName = componentType.SystemName,
                IsCommon = componentType.IsSystem,
                IsDeleted = componentType.IsDeleted
            };
            return View(model);
        }

        [CanEditAllData]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ComponentTypeEditModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var componentType = GetStorage().ComponentTypes.GetOneById(model.Id);
            CheckEditingPermissions(componentType);

            GetDispatcherClient().UpdateComponentType(new UpdateComponentTypeRequestDataDto()
            {
                Id = model.Id,
                DisplayName = model.DisplayName,
                SystemName = model.SystemName
            }).Check();

            this.SetTempMessage(TempMessageType.Success, "Тип компонента сохранён");
            return RedirectToAction("Show", new { id = model.Id });
        }

        protected void CheckEditingPermissions(ComponentTypeForRead componentType)
        {
            if (componentType.IsSystem)
                throw new UserFriendlyException($"Нельзя изменять системный тип компонента");
        }

        [CanEditAllData]
        public ActionResult Delete(Guid id)
        {
            var componentType = GetStorage().ComponentTypes.GetOneById(id);
            CheckDeletingPermissions(componentType);
            var model = new DeleteConfirmationModel()
            {
                Id = id.ToString(),
                Title = "Удаление типа компонента",
                ModalMode = Request.IsAjaxRequest(),
                Message = "Вы действительно хотите удалить этот тип компонента?",
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

            var componentType = GetStorage().ComponentTypes.GetOneById(Guid.Parse(model.Id));
            CheckDeletingPermissions(componentType);

            var client = GetDispatcherClient();
            client.DeleteComponentType(componentType.Id);

            this.SetTempMessage(TempMessageType.Success, "Тип компонента удалён");
            return Redirect(model.ReturnUrl);
        }

        protected void CheckDeletingPermissions(ComponentTypeForRead componentType)
        {
            if (componentType.IsSystem)
                throw new UserFriendlyException("Нельзя удалять системный тип компонента");
            if (componentType.IsDeleted)
                throw new UserFriendlyException("Тип компонента уже удалён");
        }

        public JsonResult CheckSystemName(ComponentTypeEditModel model)
        {
            var componentType = GetStorage().ComponentTypes.GetOneOrNullBySystemName(model.SystemName);
            if (componentType != null && (model.Id == Guid.Empty || model.Id != componentType.Id))
                return Json("Тип компонента с таким системным именем уже существует");
            return Json(true);
        }

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        internal ComponentTypesController(Guid userId) : base(userId) { }

        public ComponentTypesController() { }

    }
}