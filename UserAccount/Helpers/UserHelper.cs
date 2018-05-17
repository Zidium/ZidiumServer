using System;
using System.Linq;
using System.Web;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.Core.ConfigDb;

namespace Zidium.UserAccount
{
    public static class UserHelper
    {
        public static bool IsAdmin(this UserInfo user)
        {
            return user.Roles.Any(t => t.Id == RoleId.AccountAdministrators);
        }

        public static bool IsUser(this UserInfo user)
        {
            return user.Roles.Any(t => t.Id == RoleId.Users);
        }

        public static bool IsViewer(this UserInfo user)
        {
            return user.Roles.Any(t => t.Id == RoleId.Viewers);
        }

        public static UserInfo CurrentUser
        {
            get
            {
                if (HttpContext.Current == null)
                    return null;

                if (HttpContext.Current.User == null)
                    return null;

                if (HttpContext.Current.User.Identity == null)
                    return null;

                var userIdString = HttpContext.Current.User.Identity.Name;
                var userInfo = (HttpContext.Current.Session["CurrentUser"] as UserInfo);
                if (userInfo != null)
                {
                    // был баг, при котором после входа в систему под другим пользователем система показывала логин старого пользователя
                    if (userInfo.Id.ToString() != userIdString)
                    {
                        // вошли под другим пользователем
                        HttpContext.Current.Session.Clear();
                        userInfo = null;
                    }
                }

                if (userInfo == null)
                {
                    if (string.IsNullOrEmpty(userIdString))
                        return null;

                    Guid userId;

                    if (!Guid.TryParse(userIdString, out userId))
                        return null;

                    using (var contexts = new DatabasesContext())
                    {
                        var dbLogin = ConfigDbServicesHelper.GetLoginService().GetOneOrNullById(userId);

                        if (dbLogin == null)
                        {
                            HttpContext.Current.Session.Clear();
                            return null;
                        }

                        var service = new UserService(contexts);
                        var user = service.GetByIdOrNull(dbLogin.Account.Id, userId);

                        if (user == null)
                            return null;

                        userInfo = UserInfoByUser(user, dbLogin.Account.Id);
                        userInfo.IsSwitched = GetIsUserSwitched();

                        // обновим статистику входа
                        if (!userInfo.IsSwitched)
                        {
                            ConfigDbServicesHelper.GetLoginService().UpdateLastEntryDate(user.Id, DateTime.Now);
                        }
                    }

                    HttpContext.Current.Session["CurrentUser"] = userInfo;
                }

                return userInfo;
            }
        }

        public static UserInfo UserInfoByUser(User user, Guid accountId)
        {
            var userInfo = new UserInfo()
            {
                Id = user.Id,
                AccountId = accountId,
                Login = user.Login,
                Name = user.DisplayName,
                Roles = user.Roles.Select(t => new UserInfoRole()
                {
                    Id = t.Role.Id,
                    SystemName = t.Role.SystemName,
                    DisplayName = t.Role.DisplayName
                }).ToArray()
            };

            var account = ConfigDbServicesHelper.GetAccountService().GetOneById(accountId);
            userInfo.AccountName = account.SystemName;

            return userInfo;
        }

        public static void SetIsUserSwitched(bool isSwitched)
        {
            HttpContext.Current.Session["UserIsSwitched"] = isSwitched;
        }

        public static bool GetIsUserSwitched()
        {
            return (bool?)HttpContext.Current.Session["UserIsSwitched"] ?? false;
        }

    }
}