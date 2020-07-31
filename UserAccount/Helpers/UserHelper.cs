using System;
using System.Linq;
using System.Web;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.ConfigDb;
using Zidium.Storage;

namespace Zidium.UserAccount
{
    public static class UserHelper
    {
        public static bool IsAdmin(this UserInfo user)
        {
            return user.Roles.Any(t => t.Id == SystemRole.AccountAdministrators.Id);
        }

        public static bool IsUser(this UserInfo user)
        {
            return user.Roles.Any(t => t.Id == SystemRole.Users.Id);
        }

        public static bool IsViewer(this UserInfo user)
        {
            return user.Roles.Any(t => t.Id == SystemRole.Viewers.Id);
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
                var userInfo = HttpContext.Current.Session["CurrentUser"] as UserInfo;
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

                    var configDbServicesFactory = DependencyInjection.GetServicePersistent<IConfigDbServicesFactory>();
                    var loginInfo = configDbServicesFactory.GetLoginService().GetOneOrNullById(userId);

                    if (loginInfo == null)
                    {
                        HttpContext.Current.Session.Clear();
                        return null;
                    }

                    var accountStorageFactory = DependencyInjection.GetServicePersistent<IAccountStorageFactory>();
                    var storage = accountStorageFactory.GetStorageByAccountId(loginInfo.AccountId);
                    var user = storage.Users.GetOneOrNullById(userId);

                    if (user == null)
                        return null;

                    var roles = storage.Roles.GetByUserId(user.Id);

                    userInfo = UserInfoByUser(user, roles, loginInfo.AccountId);
                    userInfo.IsSwitched = GetIsUserSwitched();

                    // обновим статистику входа
                    if (!userInfo.IsSwitched)
                    {
                        configDbServicesFactory.GetLoginService().UpdateLastEntryDate(user.Id, DateTime.Now);
                    }

                    HttpContext.Current.Session["CurrentUser"] = userInfo;
                }

                return userInfo;
            }
        }

        public static UserInfo UserInfoByUser(UserForRead user, RoleForRead[] roles, Guid accountId)
        {
            var userInfo = new UserInfo()
            {
                Id = user.Id,
                AccountId = accountId,
                Login = user.Login,
                Name = user.DisplayName,
                Roles = roles.Select(t => new UserInfoRole()
                {
                    Id = t.Id,
                    SystemName = t.SystemName,
                    DisplayName = t.DisplayName
                }).ToArray()
            };
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