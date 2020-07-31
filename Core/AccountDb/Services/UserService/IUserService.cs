using System;
using System.Collections.Generic;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public interface IUserService
    {
        // TODO Move to auth service
        UserService.AuthInfo FindUser(string login, string accountName);

        // TODO Move to auth service
        UserService.AuthInfo Auth(string login, string password, string accountName);

        Guid CreateAccountAdmin(Guid accountId,
            string email,
            string lastName,
            string firstName,
            string middleName,
            string post,
            string mobilePhone);

        Guid CreateUser(UserForAdd user, List<UserContactForAdd> contacts, List<UserRoleForAdd> roles, Guid accountId,
            bool sendLetter = true);

        void SendNewUserLetter(Guid accountId, Guid userId, Guid token);

        void SendResetPasswordLetter(Guid accountId, Guid userId, Guid token);

        Guid StartResetPassword(Guid loginId, bool sendLetter = true);

        void EndResetPassword(Guid accountId, Guid token, string password);

        void SetNewPassword(Guid accountId, Guid userId, string newPassword);

        void UpdateUserLogin(Guid userId, string login);

        void DeleteUser(Guid userId, Guid accountId);

        Guid AddUserRole(Guid userId, UserRoleForAdd userRole);

        void RemoveUserRole(Guid userId, Guid userRoleId, Guid accountId);

        bool HasRole(Guid userId, Guid roleId);

        UserForRead GetAccountAdmin();

        UserForRead[] GetAccountAdmins();

        bool IsAccountAdmin(Guid userId);

        bool IsUser(Guid userId);

        bool IsViewer(Guid userId);
    }
}
