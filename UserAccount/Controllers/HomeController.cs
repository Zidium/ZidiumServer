using System;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Core.ConfigDb;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.Home;

namespace Zidium.UserAccount.Controllers
{
    public class HomeController : ContextController
    {
        [Authorize]
        public ActionResult Start()
        {
            var userRepository = CurrentAccountDbContext.GetUserRepository();
            var user = userRepository.GetById(CurrentUser.Id);
            var hasMobilePhone = user.UserContacts.Any(t => t.Type == UserContactType.MobilePhone && !string.IsNullOrEmpty(t.Value));

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

            var userRepository = CurrentAccountDbContext.GetUserRepository();
            var user = userRepository.GetById(CurrentUser.Id);
            userRepository.AddContactToUser(user.Id, UserContactType.MobilePhone, model.Phone, MvcApplication.GetServerDateTime());

            var client = GetDispatcherClient();

            var response = client.CreateSubscription(CurrentUser.AccountId, new CreateSubscriptionRequestData()
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
                var accountName = Core.Common.UrlHelper.GetAccountNameFromUrl(Url.ToAbsolute(Url.Current().ToString()));
                var userService = new UserService(DbContext);
                var user = userService.FindUser(model.Login, accountName);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Такого пользователя не существует");
                    return View(model);
                }

                userService.StartResetPassword(user.Id);

                return View("PasswordRestored", (object)user.Login);
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
            var tokenService = new TokenService(DbContext);
            var token = tokenService.GetOneOrNullById(accountId, id);

            if (token == null)
                return View("LinkNotValid");

            var accountDbContext = DbContext.GetAccountDbContext(accountId);
            var userRepository = accountDbContext.GetUserRepository();
            var user = userRepository.GetById(token.UserId);

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

            var tokenService = new TokenService(DbContext);
            var token = tokenService.GetOneOrNullById(model.AccountId, model.TokenId);

            if (token == null)
                return View("LinkNotValid");

            var userService = new UserService(DbContext);

            try
            {
                userService.EndResetPassword(model.AccountId, token.Id, model.Password);
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
        public HomeController(Guid? accountId, Guid? userId) : base(accountId, userId) { }

        public HomeController() { }

    }
}