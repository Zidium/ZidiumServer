using System;
using System.Collections.Generic;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public interface IUserService
    {
        // TODO Move to auth service
        UserService.AuthInfo FindUser(string login);

        // TODO Move to auth service
        UserService.AuthInfo Auth(string login, string password);

        Guid CreateAccountAdmin(string email,
            string lastName,
            string firstName,
            string middleName,
            string post,
            string mobilePhone);

        Guid CreateUser(UserForAdd user, List<UserContactForAdd> contacts, List<UserRoleForAdd> roles, bool sendLetter = true);

        void SendNewUserLetter(Guid userId, Guid token);

        void SendResetPasswordLetter(Guid userId, Guid token);

        Guid StartResetPassword(Guid loginId, bool sendLetter = true);

        void EndResetPassword(Guid token, string password);

        void SetNewPassword(Guid userId, string newPassword);

        void DeleteUser(Guid userId);

        Guid AddUserRole(Guid userId, UserRoleForAdd userRole);

        void RemoveUserRole(Guid userId, Guid userRoleId);

        bool HasRole(Guid userId, Guid roleId);

        UserForRead GetAccountAdmin();

        UserForRead[] GetAccountAdmins();

        bool IsAccountAdmin(Guid userId);

        bool IsUser(Guid userId);

        bool IsViewer(Guid userId);
    }
}
