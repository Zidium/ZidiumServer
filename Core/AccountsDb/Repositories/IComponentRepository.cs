using System;
using System.Collections.Generic;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    public interface IComponentRepository
    {
        IQueryable<Component> QueryAll();

        int GetCount();

        Component GetRoot();

        Component GetById(Guid componentId);

        Component GetByIdOrNull(Guid componentId);

        List<Component> GetAllBySystemName(string systemName);

        Component GetChild(Guid parentId, string systemName);

        Component GetBySystemName(string systemName);
        
        /// <summary>
        /// Получает список дочерних компонентов
        /// </summary>
        List<Component> GetChildComponents(Guid parentComponentId);

        IQueryable<Component> GetWhereNotActualEventsStatus();

        /// <summary>
        /// Получение всех компонентов, в том числе и удалённых
        /// Нужно для операций над удалёнными компонентами
        /// </summary>
        IQueryable<Component> QueryAllWithDeleted();
    }
}
