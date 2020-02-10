using System;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Репозиторий для работы с пользователями
    /// </summary>
    public interface IUserRepository : IAccountBasedRepository<User>
    {
        User GetOneOrNullByLogin(string login);

        /// <summary>
        /// Получение контакта пользователя по Id
        /// </summary>
        UserContact GetContactById(Guid id);

        /// <summary>
        /// Добавление контакта пользователю
        /// </summary>
        UserContact AddContactToUser(Guid userId, UserContactType type, string value, DateTime createDate);

        /// <summary>
        /// Изменение данных контакта пользователя
        /// </summary>
        UserContact EditContactById(Guid id, UserContactType type, string value);

        /// <summary>
        /// Удаление контакта пользователя по Id
        /// </summary>
        void DeleteContactById(Guid id);

        /// <summary>
        /// Получение администратора аккаунта
        /// </summary>
        User GetAccountAdmin();
    }
}
