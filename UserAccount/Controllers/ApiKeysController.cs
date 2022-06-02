using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zidium.Common;
using Zidium.Core.Api;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.ApiKeys;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class ApiKeysController : BaseController
    {
        public ApiKeysController(ILogger<ApiKeysController> logger) : base(logger)
        {
        }

        public IActionResult Index()
        {
            var apiKeys = GetDispatcherClient().GetApiKeys().Data.AsEnumerable();

            // Обычный пользователь может видеть только свои ключи
            if (!CurrentUser.CanManageAccount())
            {
                apiKeys = apiKeys.Where(t => t.UserId == CurrentUser.Id);
            }

            var model = new IndexModel()
            {
                ApiKeys = apiKeys
                    .OrderBy(t => t.Name)
                    .Select(t => new IndexModel.ApiKeyInfo()
                    {
                        Id = t.Id,
                        Name = t.Name,
                        UpdatedAt = t.UpdatedAt,
                        User = t.User
                    }).ToArray()
            };

            return View(model);
        }

        [CanManageAccount]
        public ActionResult Add()
        {
            var model = new AddModel()
            {
                ModalMode = Request.IsAjaxRequest(),
                ReturnUrl = Url.Action("Index"),
                Value = Guid.NewGuid().ToString()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CanManageAccount]
        public ActionResult Add(AddModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var data = new AddApiKeyRequestData()
            {
                Id = Ulid.NewUlid(),
                Name = model.Name,
                Value = model.Value,
                UserId = model.UserId
            };

            GetDispatcherClient().AddApiKey(data);

            var apiKey = GetDispatcherClient().GetApiKeyById(data.Id).GetDataAndCheck();

            var apiKeyModel = new IndexModel.ApiKeyInfo()
            {
                Id = apiKey.Id,
                Name = apiKey.Name,
                UpdatedAt = apiKey.UpdatedAt,
                User = apiKey.User
            };

            if (model.ModalMode)
                return PartialView("IndexRow", apiKeyModel);

            return Redirect(model.ReturnUrl);
        }

        [CanManageAccount]
        public ActionResult Edit(Guid id)
        {
            var apiKey = GetDispatcherClient().GetApiKeyById(id).GetDataAndCheck();

            var model = new EditModel()
            {
                Id = apiKey.Id,
                Name = apiKey.Name,
                Value = apiKey.Value,
                UserId = apiKey.UserId,
                ModalMode = Request.IsAjaxRequest(),
                ReturnUrl = Url.Action("Index")
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CanManageAccount]
        public ActionResult Edit(EditModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var data = new UpdateApiKeyRequestData()
            {
                Id = model.Id,
                Name = model.Name,
                UserId = model.UserId
            };

            GetDispatcherClient().UpdateApiKey(data);

            var apiKey = GetDispatcherClient().GetApiKeyById(model.Id).GetDataAndCheck();

            var apiKeyModel = new IndexModel.ApiKeyInfo()
            {
                Id = apiKey.Id,
                Name = apiKey.Name,
                UpdatedAt = apiKey.UpdatedAt,
                User = apiKey.User
            };

            if (model.ModalMode)
                return PartialView("IndexRowData", apiKeyModel);

            return Redirect(model.ReturnUrl);
        }

        [CanManageAccount]
        public ActionResult Delete(Guid id)
        {
            var apiKey = GetDispatcherClient().GetApiKeyById(id).GetDataAndCheck();

            var model = new DeleteConfirmationModel()
            {
                Id = id.ToString(),
                Title = "Удаление ключа доступа к Api",
                ModalMode = Request.IsAjaxRequest(),
                Message = "Вы действительно хотите удалить ключ " + apiKey.Name + "?",
                ReturnUrl = Url.Action("Index"),
                AjaxUpdateTargetId = "uct_" + id,
                OnAjaxSuccess = "HideModal"
            };
            return View("~/Views/Shared/Dialogs/DeleteConfirmation.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CanManageAccount]
        public ActionResult Delete(DeleteConfirmationModel model)
        {
            var apiKey = GetDispatcherClient().GetApiKeyById(Guid.Parse(model.Id)).GetDataAndCheck();

            GetDispatcherClient().DeleteApiKey(apiKey.Id);

            if (model.ModalMode)
                return new EmptyResult();

            return Redirect(model.ReturnUrl);
        }

        public ActionResult Show(Guid id)
        {
            var apiKey = GetDispatcherClient().GetApiKeyById(id).GetDataAndCheck();

            // Обычный пользователь может видеть только свои ключи
            if (!CurrentUser.CanManageAccount() && CurrentUser.Id != apiKey.UserId)
            {
                throw new NoAccessToPageException();
            }

            var model = new ShowModel()
            {
                Id = apiKey.Id,
                Name = apiKey.Name,
                Value = apiKey.Value,
                ModalMode = Request.IsAjaxRequest(),
                ReturnUrl = Url.Action("Index")
            };

            return View(model);
        }
    }
}
