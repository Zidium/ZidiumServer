using System;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Базовый интерфейс для сущностей аккаунта
    /// </summary>
    public interface IAccountBasedRepository<T>
    {
        /// <summary>
        /// Добавление новой сущности
        /// </summary>
        T Add(T entity);

        /// <summary>
        /// Поиск сущности по ключу.
        /// Если объекта нет - исключение ObjectNotFound.
        /// Если объект из другого аккаунта - исключение AccessDeniedException.
        /// Значение возвращается всегда, результат не бывает null.
        /// </summary>
        T GetById(Guid id);

        T GetByIdOrNull(Guid id);

        /// <summary>
        /// Получение всех доступных сущностей аккаунта
        /// </summary>
        IQueryable<T> QueryAll();

        /// <summary>
        /// Удаление сущности
        /// </summary>
        void Remove(T entity);

        /// <summary>
        /// Удаление объекта по id.
        /// Если объекта нет - исключение ObjectNotFound.
        /// Если объект из другого аккаунта - исключение AccessDeniedException.
        /// </summary>
        void Remove(Guid id);
    }
}
