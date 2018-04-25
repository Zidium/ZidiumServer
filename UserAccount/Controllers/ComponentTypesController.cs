using System;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class ComponentTypesController : ContextController
    {
        public ActionResult Index(string search = null)
        {
            var repository = CurrentAccountDbContext.GetComponentTypeRepository();

            var query = repository.QueryAll();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(t => t.SystemName.Contains(search) || t.DisplayName.Contains(search) || t.Id.ToString().Equals(search));

            var model = new ComponentTypesListModel
            {
                Search = search, 
                ComponentTypes = query.OrderBy(t => t.DisplayName)
            };
            return View(model);
        }

        public ActionResult Show(Guid id)
        {
            var repository = CurrentAccountDbContext.GetComponentTypeRepository();
            var componentType = repository.GetById(id);
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
            var response = client.GetOrCreateComponentType(CurrentUser.AccountId, new GetOrCreateComponentTypeRequestData()
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
            var repository = CurrentAccountDbContext.GetComponentTypeRepository();
            var componentType = repository.GetById(id);
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
            var repository = CurrentAccountDbContext.GetComponentTypeRepository();
            var componentType = repository.GetById(model.Id);
            CheckEditingPermissions(componentType);
            repository.Update(componentType, model.DisplayName, model.SystemName);
            this.SetTempMessage(TempMessageType.Success, "Тип компонента сохранён");
            return RedirectToAction("Show", new { id = model.Id });
        }

        protected void CheckEditingPermissions(ComponentType componentType)
        {
            if (componentType.IsSystem)
                throw new CantEditSystemObjectException(componentType.Id, Naming.ComponentType);
        }

        [CanEditAllData]
        public ActionResult Delete(Guid id)
        {
            var repository = CurrentAccountDbContext.GetComponentTypeRepository();
            var componentType = repository.GetById(id);
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
            var repository = CurrentAccountDbContext.GetComponentTypeRepository();
            var componentType = repository.GetById(Guid.Parse(model.Id));
            CheckDeletingPermissions(componentType);

            var client = GetDispatcherClient();
            client.DeleteComponentType(CurrentUser.AccountId, componentType.Id);

            this.SetTempMessage(TempMessageType.Success, "Тип компонента удалён");
            return Redirect(model.ReturnUrl);
        }

        protected void CheckDeletingPermissions(ComponentType componentType)
        {
            if (componentType.IsSystem)
                throw new CantDeleteSystemObjectException(componentType.Id, Naming.ComponentType);
            if (componentType.IsDeleted)
                throw new AlreadyDeletedException(componentType.Id, Naming.ComponentType);
        }

        public JsonResult CheckSystemName(ComponentTypeEditModel model)
        {
            var repository = CurrentAccountDbContext.GetComponentTypeRepository();
            var componentType = repository.GetOneOrNullBySystemName(model.SystemName);
            if (componentType != null && (model.Id == Guid.Empty || model.Id != componentType.Id))
                return Json("Тип компонента с таким системным именем уже существует", JsonRequestBehavior.AllowGet);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="userId"></param>
        public ComponentTypesController(Guid accountId, Guid userId) : base(accountId, userId) { }

        public ComponentTypesController() { }

    }
}