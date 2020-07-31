using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Компоненты
    /// </summary>
    public class ComponentForRead
    {
        public ComponentForRead(
            Guid id, 
            DateTime createdDate, 
            string displayName, 
            string systemName, 
            Guid? parentId, 
            Guid componentTypeId, 
            Guid internalStatusId, 
            Guid externalStatusId, 
            Guid unitTestsStatusId, 
            Guid eventsStatusId, 
            Guid metricsStatusId, 
            Guid childComponentsStatusId, 
            string version, 
            bool isDeleted, 
            bool enable, 
            DateTime? disableToDate, 
            string disableComment, 
            bool parentEnable)
        {
            Id = id;
            CreatedDate = createdDate;
            DisplayName = displayName;
            SystemName = systemName;
            ParentId = parentId;
            ComponentTypeId = componentTypeId;
            InternalStatusId = internalStatusId;
            ExternalStatusId = externalStatusId;
            UnitTestsStatusId = unitTestsStatusId;
            EventsStatusId = eventsStatusId;
            MetricsStatusId = metricsStatusId;
            ChildComponentsStatusId = childComponentsStatusId;
            Version = version;
            IsDeleted = isDeleted;
            Enable = enable;
            DisableToDate = disableToDate;
            DisableComment = disableComment;
            ParentEnable = parentEnable;
        }

        public Guid Id { get; }

        /// <summary>
        /// Дата и время создания компонента
        /// </summary>
        public DateTime CreatedDate { get; }

        /// <summary>
        /// Отображаемое название компонента
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Внутреннее название компонента
        /// </summary>
        public string SystemName { get; }

        /// <summary>
        /// Ссылка на родителя
        /// </summary>
        public Guid? ParentId { get; }

        /// <summary>
        /// Ссылка на тип компонента
        /// </summary>
        public Guid ComponentTypeId { get; }

        #region Статусы

        /// <summary>
        /// Внутренний статус компонента.
        /// Учитываются проверки, метрики, события. 
        /// НЕ учитываются дочерние компоненты.
        /// </summary>
        public Guid InternalStatusId { get; }

        /// <summary>
        /// Итоговый статус компонента.
        /// Учитываются проверки, метрики, события и дочерние компоненты (все учитывается)
        /// </summary>
        public Guid ExternalStatusId { get; }

        /// <summary>
        /// Статус контрольного списка проверок
        /// </summary>
        public Guid UnitTestsStatusId { get; }

        /// <summary>
        /// Статус всех пользовательских событий
        /// </summary>
        public Guid EventsStatusId { get; }

        /// <summary>
        /// Статус всех метрик
        /// </summary>
        public Guid MetricsStatusId { get; }

        /// <summary>
        /// Статус дочерних компонентов
        /// </summary>
        public Guid ChildComponentsStatusId { get; }

        #endregion

        /// <summary>
        /// Версия компонента
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public bool IsDeleted { get; }

        /// <summary>
        /// Признак что компонент включен
        /// </summary>
        public bool Enable { get; }

        /// <summary>
        /// Дата до которой выключен компонент
        /// </summary>
        public DateTime? DisableToDate { get; }

        /// <summary>
        /// Комментарий к компоненту (указывается при выключении)
        /// </summary>
        public string DisableComment { get; }

        /// <summary>
        /// Признак, что родитель включен
        /// </summary>
        public bool ParentEnable { get; }

        public ComponentForUpdate GetForUpdate()
        {
            return new ComponentForUpdate(Id);
        }
    }
}
