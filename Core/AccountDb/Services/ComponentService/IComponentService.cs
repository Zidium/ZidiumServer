using System;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public interface IComponentService
    {
        void CreateRoot(Guid accountId, Guid rootId);

        ComponentForRead GetRootComponent(Guid accountId);

        IComponentCacheReadObject GetComponentById(Guid accountId, Guid id);

        ComponentForRead GetComponentByIdNoCache(Guid accountId, Guid id);

        ComponentForRead GetComponentBySystemName(Guid accountId, Guid parentId, string systemName);

        ComponentForRead CreateComponent(Guid accountId, CreateComponentRequestData data);

        ComponentForRead GetOrCreateComponent(Guid accountId, GetOrCreateComponentRequestData data);

        void UpdateComponent(Guid accountId, UpdateComponentRequestData data);

        ComponentForRead[] GetChildComponents(Guid parentComponentId);

        /// <summary>
        /// Выключает компонент. После выключения все входящие события компонента игнорируются.
        /// </summary>
        void DisableComponent(Guid accountId, Guid componentId, DateTime? toDate, string comment);

        /// <summary>
        /// Включает компонент. Если компонент уже включен, то ничего не делает.
        /// </summary>
        void EnableComponent(Guid accountId, Guid componentId);

        /// <summary>
        /// Получает информацию о статусе компонента
        /// </summary>
        IBulbCacheReadObject GetComponentInternalState(Guid accountId, Guid componentId, bool recalc = false);

        IBulbCacheReadObject GetComponentExternalState(Guid accountId, Guid componentId, bool recalc = false);

        /// <summary>
        /// Перерасчет внутреннего статуса компонента
        /// </summary>
        IComponentCacheReadObject CalculateAllStatuses(Guid accountId, Guid componentId);

        void CalculateEventsStatus(Guid accountId, Guid componentId);

        /// <summary>
        /// Обновляет внутренний статус компонента по событию
        /// </summary>
        void ProcessEvent(Guid accountId, Guid componentId, IEventCacheReadObject eventObj);

        /// <summary>
        /// Удаление компонента и всех дочерних сущностей
        /// </summary>
        void DeleteComponent(Guid accountId, Guid componentId);

        /// <summary>
        /// Актуализирует статусы событий
        /// </summary>
        int UpdateEventsStatuses(Guid accountId, int maxCount);

        /// <summary>
        /// Получает список Id компонента и всех его детей рекурсивно
        /// Для использования в различных запросах
        /// </summary>
        Guid[] GetComponentAndChildIds(Guid accountId, Guid componentId);

        string GetFullDisplayName(ComponentForRead component);

    }
}
