using System;

namespace Zidium.Core.AccountsDb
{
    public interface IComponentTypeRepository : IAccountBasedRepository<ComponentType>
    {
        /// <summary>
        /// Добавления типа компонента с указанными параметрами
        /// </summary>
        /// <param name="displayName">Отображаемое имя</param>
        /// <param name="systemName">Системное имя</param>
        ComponentType Add(string displayName, string systemName);

        /// <summary>
        /// Получение типа компонента по системному имени
        /// </summary>
        /// <param name="systemName"></param>
        ComponentType GetOneOrNullBySystemName(string systemName);

        /// <summary>
        /// Обновление типа компонента по системному имени
        /// </summary>
        /// <param name="systemName"></param>
        /// <param name="displayName"></param>
        ComponentType UpdateBySystemName(string systemName, string displayName);

        ComponentType Update(ComponentType componentType, string displayName, string systemName);

        ComponentType Update(Guid componentTypeId, string displayName, string systemName);

    }
}
