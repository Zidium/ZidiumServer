using System;
using System.Web.Mvc;
using System.Web.Security;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.ConfigDb;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Controllers
{
    public class AccountController : ContextController
    {
        public ActionResult Logon(string returnUrl)
        {
            var model = new LogonModel()
            {
                RememberMe = true,
                ReturnUrl = returnUrl ?? Url.Action("Start", "Home"),
                ErrorMessage = TempData["error"] as string
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logon(LogonModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var accountName = GetAccountName();

            var userService = new UserService(DbContext);

            try
            {
                var authInfo = userService.Auth(model.UserName, model.Password, accountName);

                var tokenService = new TokenService(DbContext);
                var token = tokenService.GenerateToken(authInfo.AccountId, authInfo.User.Id, TokenPurpose.Logon, TimeSpan.FromMinutes(1));

                var account = ConfigDbServicesHelper.GetAccountService().GetOneById(authInfo.AccountId);

                var currentUrl = Url.ToAbsolute(Url.Current().ToString());
                var logonUrl = Url.Action("LogonByToken", new { id = token.Id, accountId = account.Id, rememberMe = model.RememberMe, returnUrl = model.ReturnUrl });
                var url = Core.Common.UrlHelper.GetAccountWebsiteUrl(account.SystemName, logonUrl, currentUrl);
                return Redirect(url);
            }
            catch (UserFriendlyException e)
            {
                // Общее событие
                MvcApplication.ComponentControl.CreateComponentEvent("LogonFailure", "Неудачный вход")
                    .SetJoinInterval(TimeSpan.FromDays(1))
                    .SetJoinKey(DateTime.Now.ToString("ddMMyyyy"))
                    .Add();

                // Индивидуальное событие для каждого логина
                MvcApplication.ComponentControl.CreateComponentEvent("LogonFailure", "Неудачный вход - " + model.UserName)
                    .SetJoinInterval(TimeSpan.FromDays(1))
                    .SetJoinKey(DateTime.Now.ToString("ddMMyyyy"), model.UserName)
                    .Add();

                ModelState.AddModelError(string.Empty, e.Message);

                return View(model);
            }
        }

        [AllowAnonymous]
        public ActionResult LogonByToken(Guid id, Guid accountId, string returnUrl = null, bool rememberMe = false, bool isSwitched = false)
        {
            var tokenService = new TokenService(DbContext);
            try
            {
                var token = tokenService.UseToken(accountId, id, TokenPurpose.Logon);

                Session.Clear();
                FormsAuthentication.SetAuthCookie(token.UserId.ToString(), rememberMe);
                UserHelper.SetIsUserSwitched(isSwitched);

                return Redirect(returnUrl ?? Url.Action("Index", "ComponentTree"));
            }
            catch (TokenNotValidException exception)
            {
                TempData["error"] = exception.Message;
                return RedirectToAction("Logon");
            }
        }

        private string GetAccountName()
        {
            var requestUrl = Url.ToAbsolute(Url.Current().ToString());
            var accountName = ConfigDbServicesHelper.GetUrlService().GetAccountNameFromUrl(requestUrl);
            return accountName;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Start", "Home");
        }

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        public AccountController(Guid accountId, Guid userId) : base(accountId, userId) { }

        public AccountController() { }

    }
}