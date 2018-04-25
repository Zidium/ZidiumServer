using System;
using Zidium.Core.Api;

namespace Zidium.Core.AccountsDb
{
    public interface IComponentTypeService
    {
        /// <summary>
        /// Регистрирует тип компонента
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        ComponentType GetOrCreateComponentType(Guid accountId, GetOrCreateComponentTypeRequestData data);

        /// <summary>
        /// Проверяет есть ли в БД тип компонента
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="componentTypeId"></param>
        /// <returns></returns>
        bool Contains(Guid accountId, Guid componentTypeId);

        ComponentType GetOneOrNullBySystemName(Guid accountId, string systemName);

        ComponentType GetOneOrNullById(Guid accountId, Guid typeId);

        ComponentType GetBySystemName(Guid accountId, string systemName);

        ComponentType GetById(Guid accountId, Guid typeId);

        ComponentType UpdateComponentType(Guid accountId, UpdateComponentTypeData data);

        void Delete(Guid accountId, Guid typeId);
    }
}
