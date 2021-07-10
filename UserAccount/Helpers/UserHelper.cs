using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Storage;

namespace Zidium.UserAccount.Helpers
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

        public static UserInfo CurrentUser(HttpContext httpContext)
        {
            if (httpContext == null)
                return null;

            if (httpContext.User == null)
                return null;

            if (httpContext.User.Identity == null)
                return null;

            var userIdString = httpContext.User.Identity.Name;
            var currentUserJson = httpContext.Session.GetString("CurrentUser");
            var userInfo = currentUserJson != null ? JsonConvert.DeserializeObject<UserInfo>(currentUserJson) : null;
            if (userInfo != null)
            {
                // был баг, при котором после входа в систему под другим пользователем система показывала логин старого пользователя
                if (userInfo.Id.ToString() != userIdString)
                {
                    // вошли под другим пользователем
                    httpContext.Session.Clear();
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

                var storageFactory = DependencyInjection.GetServicePersistent<IDefaultStorageFactory>();
                var storage = storageFactory.GetStorage();
                var user = storage.Users.GetOneOrNullById(userId);

                if (user == null)
                    return null;

                var roles = storage.Roles.GetByUserId(user.Id);

                userInfo = UserInfoByUser(user, roles);
                userInfo.IsSwitched = GetIsUserSwitched(httpContext);

                // обновим статистику входа
                if (!userInfo.IsSwitched)
                {
                    // TODO Set last logon date to user
                }

                httpContext.Session.SetString("CurrentUser", JsonConvert.SerializeObject(userInfo));
            }

            return userInfo;
        }

        public static UserInfo UserInfoByUser(UserForRead user, RoleForRead[] roles)
        {
            var userInfo = new UserInfo()
            {
                Id = user.Id,
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

        public static void SetIsUserSwitched(HttpContext httpContext, bool isSwitched)
        {
            httpContext.Session.SetString("UserIsSwitched", isSwitched.ToString());
        }

        public static bool GetIsUserSwitched(HttpContext httpContext)
        {
            var value = httpContext.Session.GetString("UserIsSwitched");
            if (bool.TryParse(value, out var result))
                return result;
            return false;
        }

    }
}