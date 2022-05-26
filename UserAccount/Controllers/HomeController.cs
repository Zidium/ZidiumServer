using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zidium.Common;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Storage;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.Home;

namespace Zidium.UserAccount.Controllers
{
    public class HomeController : BaseController
    {
        [Authorize]
        public ActionResult Start()
        {
            var hasMobilePhone = GetStorage().UserContacts
                .GetByType(CurrentUser.Id, UserContactType.MobilePhone)
                .Any(t => !string.IsNullOrEmpty(t.Value));

            var model = new StartModel()
            {
                HintSetMobilePhone = !hasMobilePhone && CurrentUser.CanEditPrivateData()
            };
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Error1()
        {
            throw new Exception("Test exception");
        }

        [AllowAnonymous]
        public ActionResult Error2()
        {
            throw new UserFriendlyException("Test user exception");
        }

        [AllowAnonymous]
        public ActionResult Error404()
        {
            return View("~/Views/Errors/Error404.cshtml");
        }

        [AllowAnonymous]
        public ActionResult EmptyPage()
        {
            return View();
        }

        [Authorize]
        public ActionResult QuickSetMobilePhone()
        {
            var model = new QuickSetMobilePhoneModel()
            {
                ReturnUrl = Referrer
            };
            return PartialView(model);
        }

        [Authorize]
        [HttpPost]
        [CanEditPrivateData]
        public ActionResult QuickSetMobilePhone(QuickSetMobilePhoneModel model)
        {
            if (!ModelState.IsValid)
                return PartialView(model);

            var userContact = new UserContactForAdd()
            {
                Id = Ulid.NewUlid(),
                UserId = CurrentUser.Id,
                Type = UserContactType.MobilePhone,
                Value = model.Phone,
                CreateDate = Now()
            };
            GetStorage().UserContacts.Add(userContact);

            var client = GetDispatcherClient();

            var response = client.CreateSubscription(new CreateSubscriptionRequestData()
            {
                UserId = CurrentUser.Id,
                Channel = SubscriptionChannel.Sms,
                Object = SubscriptionObject.Default,
                IsEnabled = true
            });
            response.Check();

            this.SetTempMessage(TempMessageType.Success, "Мобильный телефон успешно добавлен. Теперь вы будете получать уведомления по SMS!");

            return Redirect(model.ReturnUrl ?? Url.Action("Start"));
        }

        /// <summary>
        /// Просит ввести логин пользователя
        /// </summary>
        [AllowAnonymous]
        public ActionResult LostPassword()
        {
            var model = new LostPasswordModel();
            return View(model);
        }

        /// <summary>
        /// Отправляет пользователю письмо со ссылкой сброса пароля
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult LostPassword(LostPasswordModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var userService = new UserService(GetStorage(), TimeService);
                var authInfo = userService.FindUser(model.Login);

                if (authInfo != null)
                {
                    userService.StartResetPassword(authInfo.User.Id);
                }

                return View("PasswordRestored");
            }
            catch (UserFriendlyException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return View(model);
            }
        }

        /// <summary>
        /// На данный url приходят пользователи из письма со сменой пароля
        /// Страница просит пользователя ввести новый пароль
        /// </summary>
        [AllowAnonymous]
        public ActionResult SetPassword(Guid id, Guid accountId)
        {
            var storage = GetStorage();

            var tokenService = new TokenService(storage, TimeService);
            var token = tokenService.GetOneOrNullById(id);

            if (token == null)
                return View("LinkNotValid");

            var user = storage.Users.GetOneById(token.UserId);

            var model = new SetPasswordModel()
            {
                TokenId = id,
                UserName = user.DisplayName,
                AccountId = accountId
            };

            return View(model);
        }

        /// <summary>
        /// Устанавливает новый пароль по токену (из письма)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult SetPassword(SetPasswordModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var storage = GetStorage();

            var tokenService = new TokenService(storage, TimeService);
            var token = tokenService.GetOneOrNullById(model.TokenId);

            if (token == null)
                return View("LinkNotValid");

            var userService = new UserService(storage, TimeService);

            try
            {
                userService.EndResetPassword(token.Id, model.Password);
            }
            catch (TokenNotValidException)
            {
                return View("LinkNotValid");
            }
            catch (UserFriendlyException e)
            {
                ModelState.AddModelError("", e.Message);
                return View(model);
            }

            return View("PasswordChanged");
        }

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        internal HomeController(Guid? userId) : base(userId) { }

        public HomeController(ILogger<HomeController> logger) : base(logger)
        {
        }
    }
}