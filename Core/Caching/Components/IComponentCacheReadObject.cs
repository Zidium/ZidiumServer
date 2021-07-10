using System;

namespace Zidium.Core.Caching
{
    public interface IComponentCacheReadObject : IAccountDbCacheReadObject
    {
        /// <summary>
        /// Дата и время создания компонента
        /// </summary>
        DateTime CreatedDate { get; }

        /// <summary>
        /// Отображаемое название компонента
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Внутреннее название компонента
        /// </summary>
        string SystemName { get; }

        /// <summary>
        /// Ссылка на родителя
        /// </summary>
        Guid? ParentId { get; }

        /// <summary>
        /// Ссылка на тип компонента
        /// </summary>
        Guid ComponentTypeId { get; }

        /// <summary>
        /// Коллекция дочерних компонентов
        /// </summary>
        IReadOnlyReferenceCollection Childs { get; }

        /// <summary>
        /// Юнит-тесты
        /// </summary>
        IReadOnlyReferenceCollection UnitTests { get; }

        /// <summary>
        /// Метрики компонента
        /// </summary>
        IReadOnlyReferenceCollection Metrics { get; }

        #region Колбаски

        Guid InternalStatusId { get; }

        Guid ExternalStatusId { get; }

        Guid UnitTestsStatusId { get; }

        Guid EventsStatusId { get; }

        Guid MetricsStatusId { get; }

        Guid ChildComponentsStatusId { get; }

        #endregion

        /// <summary>
        /// Признак корневого компонента
        /// </summary>
        bool IsRoot { get; }

        /// <summary>
        /// Признак папки
        /// </summary>
        bool IsFolder { get; }

        /// <summary>
        /// Версия компонента
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        bool IsDeleted { get; }

        /// <summary>
        /// Признак что компонент включен
        /// </summary>
        bool Enable { get; }

        /// <summary>
        /// Дата до которой выключен компонент
        /// </summary>
        DateTime? DisableToDate { get; }

        /// <summary>
        /// Комментарий к компоненту (указывается при выключении)
        /// </summary>
        string DisableComment { get; }

        /// <summary>
        /// Признак, что родитель включен
        /// </summary>
        bool ParentEnable { get; }

        /// <summary>
        /// Компонент и его родитель включены
        /// </summary>
        bool CanProcess { get; }

        Guid[] GetAllStatusesIds();
    }
}
