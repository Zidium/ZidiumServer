using System;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Базовый интерфейс для сущностей хранилища
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IComponentBasedRepository<T>
    {
        /// <summary>
        /// Добавление новой сущности
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        T Add(T entity);

        /// <summary>
        /// Поиск сущности по ключу.
        /// Если объекта нет - исключение ObjectNotFound.
        /// Если объект из другого компонента - исключение AccessDeniedException.
        /// Значение возвращается всегда, результат не бывает null.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="componentId"></param>
        /// <returns></returns>
        T Find(Guid id, Guid componentId);

        /// <summary>
        /// Получение всех доступных сущностей компонента
        /// </summary>
        /// <param name="componentId"></param>
        /// <returns></returns>
        IQueryable<T> QueryAll(Guid componentId);

        /// <summary>
        /// Удаление сущности
        /// </summary>
        /// <param name="entity"></param>
        void Remove(T entity);

        /// <summary>
        /// Удаление объекта по id.
        /// Если объекта нет - исключение ObjectNotFound.
        /// Если объект из другого компонента - исключение AccessDeniedException.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="componentId"></param>
        void Remove(Guid id, Guid componentId);
    }
}
