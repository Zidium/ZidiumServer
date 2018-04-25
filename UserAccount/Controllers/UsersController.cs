using System;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.ConfigDb;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.Others;
using Zidium.UserAccount.Models.Users;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class UsersController : ContextController
    {
        /// <summary>
        /// Вывод списка пользователей
        /// </summary>
        [CanManageAccount]
        public ActionResult Index()
        {
            var repository = CurrentAccountDbContext.GetUserRepository();
            var usersList = repository.QueryAll().OrderBy(x => x.Login);
            return View(usersList);
        }

        public ActionResult Show(Guid id)
        {
            var repository = CurrentAccountDbContext.GetUserRepository();
            var user = repository.GetById(id);

            if (!CurrentUser.CanManageAccount() && user.Id != CurrentUser.Id)
                throw new NoAccessToPageException();

            var model = new ShowUserModel()
            {
                Id = user.Id,
                Login = user.Login,
                DisplayName = user.DisplayName,
                Contacts = user.UserContacts.ToList(),
                Role = user.Roles.Any() ? user.Roles.First().Role.DisplayName : "нет"
            };

            var userSettingService = CurrentAccountDbContext.GetUserSettingService();
            model.SendMeNews = userSettingService.SendMeNews(user.Id);

            return View(model);
        }

        /// <summary>
        /// Вывод формы добавления пользователя
        /// </summary>
        [CanManageAccount]
        public ActionResult Add()
        {
            var user = new AddUserModel();
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
                    var modelState = new ModelStateHelper<AddUserModel>(ModelState);
                    modelState.AddErrorFor(x=>x.DisplayName, "Значение не должно быть пустым");
                }
            }

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var user = new User()
                {
                    Login = model.Login,
                    DisplayName = model.DisplayName,
                    Post = model.Post
                };

                var userService = new UserService(DbContext);

                // обновим роль
                if (model.RoleId.HasValue)
                {
                    userService.AddUserRole(user, new UserRole()
                    {
                        RoleId = model.RoleId.Value
                    });
                }

                userService.CreateUser(user, CurrentUser.AccountId);
                DbContext.SaveChanges();
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
            var repository = CurrentAccountDbContext.GetUserRepository();
            var user = repository.GetById(id);

            if (!CurrentUser.CanManageAccount() && user.Id != CurrentUser.Id)
                throw new NoAccessToPageException();

            var model = new EditUserModel()
            {
                Id = user.Id,
                Login = user.Login,
                DisplayName = user.DisplayName,
                Post = user.Post,
                RoleId = user.Roles.Any() ? user.Roles.First().RoleId : (Guid?) null,
                Contacts = user.UserContacts.ToList()
            };

            var userSettingService = CurrentAccountDbContext.GetUserSettingService();
            model.SendMeNews = userSettingService.SendMeNews(user.Id);

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
            var userService = new UserService(DbContext);
            var user = userService.GetById(CurrentUser.AccountId, model.Id);

            if (!CurrentUser.CanManageAccount() && user.Id != CurrentUser.Id)
                throw new NoAccessToPageException();

            model.Contacts = user.UserContacts.ToList();

            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(model.DisplayName))
                {
                    var modelState = new ModelStateHelper<EditUserModel>(ModelState);
                    modelState.AddErrorFor(x => x.DisplayName, "Значение не должно быть пустым");
                }
            }

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                user.Login = model.Login;
                user.DisplayName = model.DisplayName;
                user.Post = model.Post;

                // обновим роль, если она поменялась
                if (CurrentUser.CanManageAccount())
                {
                    if (user.Roles.First().RoleId != model.RoleId)
                    {
                        foreach (var role in user.Roles.ToArray())
                        {
                            userService.RemoveUserRole(user, role, CurrentUser.AccountId);
                        }

                        if (model.RoleId.HasValue)
                            userService.AddUserRole(user, new UserRole()
                            {
                                RoleId = model.RoleId.Value
                            });
                    }
                }

                userService.UpdateUserLogin(user);

                var userSettingService = CurrentAccountDbContext.GetUserSettingService();
                userSettingService.SendMeNews(user.Id, model.SendMeNews);

                DbContext.SaveChanges();
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
            var repository = CurrentAccountDbContext.GetUserRepository();
            var user = repository.GetById(id);
            var model = new DeleteConfirmationModel()
            {
                Id = user.Id.ToString(),
                Title = "Удаление пользователя",
                ModalMode = Request.IsAjaxRequest(),
                Message = "Вы действительно хотите удалить этого пользователя?",
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
            var userService = new UserService(DbContext);
            var user = userService.GetById(CurrentUser.AccountId, Guid.Parse(model.Id));
            userService.DeleteUser(user, CurrentUser.AccountId);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Вывод формы редактирования контакта
        /// </summary>
        [CanEditPrivateData]
        public ActionResult EditContact(Guid id)
        {
            var repository = CurrentAccountDbContext.GetUserRepository();
            var contact = repository.GetContactById(id);

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
            var repository = CurrentAccountDbContext.GetUserRepository();
            var contact = repository.GetContactById(model.Id);

            if (!CurrentUser.CanManageAccount() && contact.UserId != CurrentUser.Id)
                throw new NoAccessToPageException();

            repository.EditContactById(model.Id, model.Type, model.Value);

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
            var repository = CurrentAccountDbContext.GetUserRepository();
            var user = repository.GetById(userId);

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
            var repository = CurrentAccountDbContext.GetUserRepository();
            var userId = model.UserId;
            var user = repository.GetById(userId);

            if (!CurrentUser.CanManageAccount() && user.Id != CurrentUser.Id)
                throw new NoAccessToPageException();

            var contact = repository.AddContactToUser(user.Id, model.Type, model.Value, MvcApplication.GetServerDateTime());

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
            var repository = CurrentAccountDbContext.GetUserRepository();
            var contact = repository.GetContactById(id);

            if (!CurrentUser.CanManageAccount() && contact.UserId != CurrentUser.Id)
                throw new NoAccessToPageException();

            var model = new DeleteConfirmationModel()
            {
                Id = id.ToString(),
                Title = "Удаление контакта",
                ModalMode = Request.IsAjaxRequest(),
                Message = "Вы действительно хотите удалить этот контакт?",
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
            var repository = CurrentAccountDbContext.GetUserRepository();
            var contact = repository.GetContactById(Guid.Parse(model.Id));

            if (!CurrentUser.CanManageAccount() && contact.UserId != CurrentUser.Id)
                throw new NoAccessToPageException();

            repository.DeleteContactById(contact.Id);

            if (model.ModalMode)
                return new EmptyResult();

            return Redirect(model.ReturnUrl);
        }

        [CanEditPrivateData]
        public ActionResult ChangePasswordDialog(Guid id)
        {
            var user = GetUserById(id);
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
                var state = new ModelStateHelper<ChangePasswordModel>(ModelState);
                if (string.IsNullOrWhiteSpace(model.Password))
                {
                    state.AddErrorFor(x=>x.Password, "Не указан новый пароль");
                }
                else if (model.Password != model.PasswordConfirm)
                {
                    state.AddErrorFor(x=>x.PasswordConfirm, "Отличается от нового пароля");
                }
                if (ModelState.IsValid)
                {
                    var userService = new UserService(DbContext);
                    userService.SetNewPassword(CurrentUser.AccountId, model.UserId, model.Password);
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
            var userService = new UserService(DbContext);
            userService.StartResetPassword(id);
            var user = GetUserById(id);
            var message = $"На адрес {user.Login} отправлено письмо с одноразовой ссылкой на смену пароля";
            this.SetTempMessage(TempMessageType.Success, message);
            return RedirectToAction("Edit", new { Id = id });
        }

        public JsonResult CheckNewLogin(AddUserModel model)
        {
            var login = ConfigDbServicesHelper.GetLoginService().GetOneOrNull(CurrentUser.AccountId, model.Login);
            if (login != null)
                return Json("Пользователь с таким EMail уже существует", JsonRequestBehavior.AllowGet);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckExistingLogin(EditUserModel model)
        {
            var login = ConfigDbServicesHelper.GetLoginService().GetOneOrNull(CurrentUser.AccountId, model.Login);
            if (login != null)
            {
                using (var accountContext = AccountDbContext.CreateFromAccountId(login.Account.Id))
                {
                    var userRepository = accountContext.GetUserRepository();
                    var user = userRepository.GetOneOrNullByLogin(model.Login);
                    if (user != null && user.Id != model.Id)
                        return Json("Пользователь с таким EMail уже существует", JsonRequestBehavior.AllowGet);
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

    }
}