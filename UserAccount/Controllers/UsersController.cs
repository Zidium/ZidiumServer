using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zidium.Common;
using Zidium.Core.AccountsDb;
using Zidium.Storage;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.Users;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        public UsersController(ILogger<UsersController> logger) : base(logger)
        {
        }

        /// <summary>
        /// Вывод списка пользователей
        /// </summary>
        [CanManageAccount]
        public ActionResult Index()
        {
            var users = GetStorage().Users.GetAll().OrderBy(x => x.Login).ToArray();
            var model = new IndexModel()
            {
                Users = users.Select(t => new IndexModel.UserInfo()
                {
                    Id = t.Id,
                    DisplayName = t.DisplayName,
                    Login = t.Login,
                    Post = t.Post,
                    Role = GetStorage().Roles.GetByUserId(t.Id).FirstOrDefault()?.DisplayName
                }).ToArray()
            };
            return View(model);
        }

        public ActionResult Show(Guid id)
        {
            var user = GetStorage().Users.GetOneById(id);

            if (!CurrentUser.CanManageAccount() && user.Id != CurrentUser.Id)
                throw new NoAccessToPageException();

            var model = new ShowUserModel()
            {
                Id = user.Id,
                Login = user.Login,
                DisplayName = user.DisplayName,
                Contacts = GetStorage().UserContacts.GetByUserId(user.Id),
                Role = GetStorage().Roles.GetByUserId(user.Id).FirstOrDefault()?.DisplayName
            };

            var userSettingService = new UserSettingService(GetStorage());
            model.SendMeNews = userSettingService.SendMeNews(user.Id);

            var timeZoneOffsetMinutes = userSettingService.TimeZoneOffsetMinutes(user.Id);
            var timeZone = GetStorage().TimeZones.GetOneByOffsetMinutes(timeZoneOffsetMinutes);
            model.TimeZone = timeZone.Name;

            return View(model);
        }

        /// <summary>
        /// Вывод формы добавления пользователя
        /// </summary>
        [CanManageAccount]
        public ActionResult Add()
        {
            var user = new AddUserModel()
            {
                TimeZoneOffsetMinutes = 3 * 60
            };
            return View(user);
        }

        /// <summary>
        /// Добавление пользователя
        /// </summary>
        [CanManageAccount]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AddUserModel model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(model.DisplayName))
                {
                    var modelState = new ModelStateHelper<AddUserModel>(ModelState, HttpContext);
                    modelState.AddErrorFor(x => x.DisplayName, "Значение не должно быть пустым");
                }
            }

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var user = new UserForAdd()
                {
                    Id = Ulid.NewUlid(),
                    Login = model.Login,
                    DisplayName = model.DisplayName,
                    Post = model.Post
                };

                var userService = new UserService(GetStorage());

                // обновим роль
                var roles = new List<UserRoleForAdd>();

                if (model.RoleId.HasValue)
                {
                    roles.Add(new UserRoleForAdd()
                    {
                        Id = Ulid.NewUlid(),
                        UserId = user.Id,
                        RoleId = model.RoleId.Value
                    });
                }

                userService.CreateUser(user, new List<UserContactForAdd>(), roles);

                var userSettingService = new UserSettingService(GetStorage());
                userSettingService.TimeZoneOffsetMinutes(user.Id, model.TimeZoneOffsetMinutes);
            }
            catch (UserFriendlyException e)
            {
                ModelState.AddModelError("", e.Message);
                return View(model);
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Вывод формы редактирования пользователя
        /// </summary>
        [CanEditPrivateData]
        public ActionResult Edit(Guid id)
        {
            var user = GetStorage().Users.GetOneById(id);

            if (!CurrentUser.CanManageAccount() && user.Id != CurrentUser.Id)
                throw new NoAccessToPageException();

            var model = new EditUserModel()
            {
                Id = user.Id,
                Login = user.Login,
                DisplayName = user.DisplayName,
                Post = user.Post,
                RoleId = GetStorage().Roles.GetByUserId(user.Id).FirstOrDefault()?.Id,
                Contacts = GetStorage().UserContacts.GetByUserId(user.Id)
            };

            var userSettingService = new UserSettingService(GetStorage());
            model.SendMeNews = userSettingService.SendMeNews(user.Id);
            model.TimeZoneOffsetMinutes = userSettingService.TimeZoneOffsetMinutes(user.Id);

            return View(model);
        }

        /// <summary>
        /// Сохранение данных пользователя после редактирования
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CanEditPrivateData]
        public ActionResult Edit(EditUserModel model)
        {
            var user = GetStorage().Users.GetOneById(model.Id);

            if (!CurrentUser.CanManageAccount() && user.Id != CurrentUser.Id)
                throw new NoAccessToPageException();

            model.Contacts = GetStorage().UserContacts.GetByUserId(user.Id);

            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(model.DisplayName))
                {
                    var modelState = new ModelStateHelper<EditUserModel>(ModelState, HttpContext);
                    modelState.AddErrorFor(x => x.DisplayName, "Значение не должно быть пустым");
                }
            }

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var userForUpdate = user.GetForUpdate();
                userForUpdate.Login.Set(model.Login);
                userForUpdate.DisplayName.Set(model.DisplayName);
                userForUpdate.Post.Set(model.Post);
                GetStorage().Users.Update(userForUpdate);

                var userService = new UserService(GetStorage());

                // обновим роль, если она поменялась
                if (CurrentUser.CanManageAccount())
                {
                    var roles = GetStorage().UserRoles.GetByUserId(user.Id);
                    if (roles.Length > 0 && roles.First().RoleId != model.RoleId)
                    {
                        foreach (var role in roles)
                        {
                            userService.RemoveUserRole(user.Id, role.Id);
                        }

                        if (model.RoleId.HasValue)
                            userService.AddUserRole(user.Id, new UserRoleForAdd()
                            {
                                Id = Ulid.NewUlid(),
                                RoleId = model.RoleId.Value
                            });
                    }
                }

                var userSettingService = new UserSettingService(GetStorage());
                userSettingService.SendMeNews(user.Id, model.SendMeNews);
                userSettingService.TimeZoneOffsetMinutes(user.Id, model.TimeZoneOffsetMinutes);
            }
            catch (UserFriendlyException e)
            {
                ModelState.AddModelError("", e.Message);
                return View(model);
            }

            return RedirectToAction("Show", new { id = user.Id });
        }

        /// <summary>
        /// Вывод формы удаления пользователя
        /// </summary>
        [CanManageAccount]
        public ActionResult Delete(Guid id)
        {
            var user = GetStorage().Users.GetOneById(id);
            var model = new DeleteConfirmationModel()
            {
                Id = user.Id.ToString(),
                Title = "Удаление пользователя",
                ModalMode = Request.IsAjaxRequest(),
                Message = "Вы действительно хотите удалить пользователя" + user.Login + "?",
                ReturnUrl = Url.Action("Index")
            };
            return View("~/Views/Shared/Dialogs/DeleteConfirmation.cshtml", model);
        }

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        [CanManageAccount]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(DeleteConfirmationModel model)
        {
            var user = GetStorage().Users.GetOneById(Guid.Parse(model.Id));
            var userService = new UserService(GetStorage());
            userService.DeleteUser(user.Id);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Вывод формы редактирования контакта
        /// </summary>
        [CanEditPrivateData]
        public ActionResult EditContact(Guid id)
        {
            var contact = GetStorage().UserContacts.GetOneById(id);

            if (!CurrentUser.CanManageAccount() && contact.UserId != CurrentUser.Id)
                throw new NoAccessToPageException();

            var model = new UserContactModel()
            {
                ModalMode = Request.IsAjaxRequest(),
                ReturnUrl = Url.Action("Edit", new { id = contact.UserId.ToString() }),
                Id = id,
                UserId = contact.UserId,
                Type = contact.Type,
                Value = contact.Value
            };
            return View(model);
        }

        /// <summary>
        /// Редактирование контакта
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CanEditPrivateData]
        public ActionResult EditContact(UserContactModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var contact = GetStorage().UserContacts.GetOneById(model.Id);

            if (!CurrentUser.CanManageAccount() && contact.UserId != CurrentUser.Id)
                throw new NoAccessToPageException();

            var contactForUpdate = contact.GetForUpdate();
            contactForUpdate.Type.Set(model.Type);
            contactForUpdate.Value.Set(model.Value);
            GetStorage().UserContacts.Update(contactForUpdate);
            contact = GetStorage().UserContacts.GetOneById(contactForUpdate.Id);

            if (model.ModalMode)
                return PartialView("UserContactData", contact);

            return Redirect(model.ReturnUrl);
        }

        /// <summary>
        /// Вывод формы добавления контакта
        /// </summary>
        [CanEditPrivateData]
        public ActionResult AddContact(Guid userId)
        {
            var user = GetStorage().Users.GetOneById(userId);

            if (!CurrentUser.CanManageAccount() && user.Id != CurrentUser.Id)
                throw new NoAccessToPageException();

            var model = new UserContactModel()
            {
                ModalMode = Request.IsAjaxRequest(),
                ReturnUrl = Url.Action("Edit", new { id = userId.ToString() }),
                UserId = user.Id
            };
            return View(model);
        }

        /// <summary>
        /// Добавление контакта
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CanEditPrivateData]
        public ActionResult AddContact(UserContactModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = model.UserId;
            var user = GetStorage().Users.GetOneById(userId);

            if (!CurrentUser.CanManageAccount() && user.Id != CurrentUser.Id)
                throw new NoAccessToPageException();

            var contactForAdd = new UserContactForAdd()
            {
                Id = Ulid.NewUlid(),
                UserId = user.Id,
                Type = model.Type,
                Value = model.Value,
                CreateDate = Now()
            };
            GetStorage().UserContacts.Add(contactForAdd);
            var contact = GetStorage().UserContacts.GetOneById(contactForAdd.Id);

            if (model.ModalMode)
                return PartialView("UserContactRow", contact);

            return Redirect(model.ReturnUrl);
        }

        /// <summary>
        /// Вывод формы удаления контакта
        /// </summary>
        [CanEditPrivateData]
        public ActionResult DeleteContact(Guid id)
        {
            var contact = GetStorage().UserContacts.GetOneById(id);

            if (!CurrentUser.CanManageAccount() && contact.UserId != CurrentUser.Id)
                throw new NoAccessToPageException();

            var model = new DeleteConfirmationModel()
            {
                Id = id.ToString(),
                Title = "Удаление контакта",
                ModalMode = Request.IsAjaxRequest(),
                Message = "Вы действительно хотите удалить контакт" + contact.Value + "?",
                ReturnUrl = Url.Action("Edit", new { id = contact.UserId }),
                AjaxUpdateTargetId = "uct_" + id,
                OnAjaxSuccess = "HideModal"
            };
            return View("~/Views/Shared/Dialogs/DeleteConfirmation.cshtml", model);
        }

        /// <summary>
        /// Удаление контакта
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CanEditPrivateData]
        public ActionResult DeleteContact(DeleteConfirmationModel model)
        {
            var contact = GetStorage().UserContacts.GetOneById(Guid.Parse(model.Id));

            if (!CurrentUser.CanManageAccount() && contact.UserId != CurrentUser.Id)
                throw new NoAccessToPageException();

            GetStorage().UserContacts.Delete(contact.Id);

            if (model.ModalMode)
                return new EmptyResult();

            return Redirect(model.ReturnUrl);
        }

        [CanEditPrivateData]
        public ActionResult ChangePasswordDialog(Guid id)
        {
            var user = GetStorage().Users.GetOneById(id);
            var model = new ChangePasswordModel()
            {
                UserId = user.Id,
                UserName = user.DisplayName
            };
            return PartialView(model);
        }

        [CanEditPrivateData]
        [HttpPost]
        public ActionResult ChangePasswordDialog(ChangePasswordModel model)
        {
            try
            {
                var state = new ModelStateHelper<ChangePasswordModel>(ModelState, HttpContext);
                if (string.IsNullOrWhiteSpace(model.Password))
                {
                    state.AddErrorFor(x => x.Password, "Не указан новый пароль");
                }
                else if (model.Password != model.PasswordConfirm)
                {
                    state.AddErrorFor(x => x.PasswordConfirm, "Отличается от нового пароля");
                }
                if (ModelState.IsValid)
                {
                    var userService = new UserService(GetStorage());
                    userService.SetNewPassword(model.UserId, model.Password);
                    var endModel = new ChangePasswordEndModel()
                    {
                        UserId = model.UserId,
                        UserName = model.UserName
                    };
                    return PartialView("ChangePasswordDialogEnd", endModel);
                }
            }
            catch (Exception e)
            {
                model.ErrorMessage = e.Message;
            }
            return PartialView(model);
        }

        [HttpPost]
        [CanEditPrivateData]
        public ActionResult StartResetPassword(Guid id)
        {
            var userService = new UserService(GetStorage());
            userService.StartResetPassword(id);
            var user = GetStorage().Users.GetOneById(id);
            var message = $"На адрес {user.Login} отправлено письмо с одноразовой ссылкой на смену пароля";
            this.SetTempMessage(TempMessageType.Success, message);
            return RedirectToAction("Edit", new { Id = id });
        }

        public JsonResult CheckNewLogin(AddUserModel model)
        {
            var user = GetStorage().Users.GetOneOrNullByLogin(model.Login);
            if (user != null)
                return Json("Пользователь с таким EMail уже существует");
            return Json(true);
        }

        public JsonResult CheckExistingLogin(EditUserModel model)
        {
            var user = GetStorage().Users.GetOneOrNullByLogin(model.Login);
            if (user != null && user.Id != model.Id)
                return Json("Пользователь с таким EMail уже существует");
            return Json(true);
        }

    }
}