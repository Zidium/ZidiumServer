using System;

namespace Zidium.Core.AccountsDb
{
    public interface IUserService
    {
        User FindUser(string login, string accountName);

        UserService.AuthInfo Auth(string login, string password, string accountName);

        User CreateAccountAdmin(
            Guid accountId,
            string email,
            string lastName,
            string firstName,
            string middleName,
            string post,
            string mobilePhone);

        User CreateUser(User user, Guid accountId, bool sendLetter = true);

        void SendNewUserLetter(Guid accountId, Guid userId, Guid token);

        void SendResetPasswordLetter(Guid accountId, Guid userId, Guid token);

        Guid StartResetPassword(Guid loginId, bool sendLetter = true);

        void EndResetPassword(Guid accountId, Guid token, string password);

        void SetNewPassword(Guid accountId, Guid userId, string newPassword);

        User GetById(Guid accountId, Guid id);

        User GetByIdOrNull(Guid accountId, Guid id);

        User UpdateUserLogin(User user);

        void DeleteUser(User user, Guid accountId);

        UserRole AddUserRole(User user, UserRole userRole);

        void RemoveUserRole(User user, UserRole userRole, Guid accountId);
    }
}
