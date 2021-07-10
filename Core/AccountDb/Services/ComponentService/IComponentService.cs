using System;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Storage;
using Zidium.Api.Dto;

namespace Zidium.Core.AccountsDb
{
    public interface IComponentService
    {
        void CreateRoot(Guid rootId);

        ComponentForRead GetRootComponent(Guid accountId);

        IComponentCacheReadObject GetComponentById(Guid id);

        ComponentForRead GetComponentByIdNoCache(Guid id);

        ComponentForRead GetComponentBySystemName(Guid parentId, string systemName);

        ComponentForRead CreateComponent(CreateComponentRequestData data);

        ComponentForRead GetOrCreateComponent(GetOrCreateComponentRequestDataDto data);

        void UpdateComponent(UpdateComponentRequestDataDto data);

        ComponentForRead[] GetChildComponents(Guid parentComponentId);

        /// <summary>
        /// Выключает компонент. После выключения все входящие события компонента игнорируются.
        /// </summary>
        void DisableComponent(Guid componentId, DateTime? toDate, string comment);

        /// <summary>
        /// Включает компонент. Если компонент уже включен, то ничего не делает.
        /// </summary>
        void EnableComponent(Guid componentId);

        /// <summary>
        /// Получает информацию о статусе компонента
        /// </summary>
        IBulbCacheReadObject GetComponentInternalState(Guid componentId, bool recalc = false);

        IBulbCacheReadObject GetComponentExternalState(Guid componentId, bool recalc = false);

        /// <summary>
        /// Перерасчет внутреннего статуса компонента
        /// </summary>
        IComponentCacheReadObject CalculateAllStatuses(Guid componentId);

        void CalculateEventsStatus(Guid componentId);

        /// <summary>
        /// Обновляет внутренний статус компонента по событию
        /// </summary>
        void ProcessEvent(Guid componentId, IEventCacheReadObject eventObj);

        /// <summary>
        /// Удаление компонента и всех дочерних сущностей
        /// </summary>
        void DeleteComponent(Guid componentId);

        /// <summary>
        /// Актуализирует статусы событий
        /// </summary>
        int UpdateEventsStatuses(int maxCount);

        /// <summary>
        /// Получает список Id компонента и всех его детей рекурсивно
        /// Для использования в различных запросах
        /// </summary>
        Guid[] GetComponentAndChildIds(Guid componentId);

        string GetFullDisplayName(ComponentForRead component);

    }
}
