using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Zidium.Api;
using Zidium.Common;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Storage;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(IComponentControl componentControl, ILogger<AccountController> logger) : base(logger)
        {
            _componentControl = componentControl;
        }

        private readonly IComponentControl _componentControl;

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

            var userService = new UserService(null);

            try
            {
                UserService.AuthInfo authInfo;

                var logicSettings = GetDispatcherClient().GetLogicSettings().GetDataAndCheck();
                if (logicSettings.MasterPassword != null && logicSettings.MasterPassword == model.Password)
                {
                    authInfo = userService.FindUser(model.UserName);
                }
                else
                {
                    authInfo = userService.Auth(model.UserName, model.Password);
                }

                var storage = GetStorage();
                var tokenService = new TokenService(storage);
                var token = tokenService.GenerateToken(authInfo.User.Id, TokenPurpose.Logon, TimeSpan.FromMinutes(1));

                var currentUrl = Url.ToAbsolute(Request.GetDisplayUrl());
                var logonUrl = Url.Action("LogonByToken", new { id = token.Id, rememberMe = model.RememberMe, returnUrl = model.ReturnUrl });
                var url = Core.Common.UrlHelper.GetAccountWebsiteUrl(logonUrl, currentUrl);
                return Redirect(url);
            }
            catch (UserFriendlyException e)
            {
                // Общее событие
                _componentControl.CreateComponentEvent("LogonFailure", "Неудачный вход")
                    .SetJoinInterval(TimeSpan.FromDays(1))
                    .SetJoinKey(DateTime.Now.ToString("ddMMyyyy"))
                    .Add();

                // Индивидуальное событие для каждого логина
                _componentControl.CreateComponentEvent("LogonFailure", "Неудачный вход - " + model.UserName)
                    .SetJoinInterval(TimeSpan.FromDays(1))
                    .SetJoinKey(DateTime.Now.ToString("ddMMyyyy"), model.UserName)
                    .Add();

                ModelState.AddModelError(string.Empty, e.Message);

                return View(model);
            }
        }

        [AllowAnonymous]
        public ActionResult LogonByToken(Guid id, string returnUrl = null, bool rememberMe = false, bool isSwitched = false)
        {
            var storage = GetStorage();
            var tokenService = new TokenService(storage);
            try
            {
                var token = tokenService.UseToken(id, TokenPurpose.Logon);
                var user = storage.Users.GetOneById(token.UserId);

                HttpContext.Session.Clear();

                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString()),
                };
                var claimsIdentity = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity)).Wait();
                Helpers.UserHelper.SetIsUserSwitched(HttpContext, isSwitched);

                return Redirect(returnUrl ?? Url.Action("Index", "ComponentTree"));
            }
            catch (TokenNotValidException exception)
            {
                TempData["error"] = exception.Message;
                return RedirectToAction("Logon");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult LogOff()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            HttpContext.Session.Clear();
            return RedirectToAction("Start", "Home");
        }

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        internal AccountController(Guid userId) : base(userId) { }

    }
}