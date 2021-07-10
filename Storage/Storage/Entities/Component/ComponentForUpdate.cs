using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Компоненты
    /// </summary>
    public class ComponentForUpdate
    {
        public ComponentForUpdate(Guid id)
        {
            Id = id;
            DisplayName = new ChangeTracker<string>();
            SystemName = new ChangeTracker<string>();
            ParentId = new ChangeTracker<Guid?>();
            ComponentTypeId = new ChangeTracker<Guid>();
            Version = new ChangeTracker<string>();
            Enable = new ChangeTracker<bool>();
            DisableToDate = new ChangeTracker<DateTime?>();
            DisableComment = new ChangeTracker<string>();
            ParentEnable = new ChangeTracker<bool>();
            IsDeleted = new ChangeTracker<bool>();
            InternalStatusId = new ChangeTracker<Guid>();
            ExternalStatusId = new ChangeTracker<Guid>();
            UnitTestsStatusId = new ChangeTracker<Guid>();
            EventsStatusId = new ChangeTracker<Guid>();
            MetricsStatusId = new ChangeTracker<Guid>();
            ChildComponentsStatusId = new ChangeTracker<Guid>();
        }

        public Guid Id { get; }

        /// <summary>
        /// Отображаемое название компонента
        /// </summary>
        public ChangeTracker<string> DisplayName { get; }

        /// <summary>
        /// Внутреннее название компонента
        /// </summary>
        public ChangeTracker<string> SystemName { get; }

        /// <summary>
        /// Ссылка на родителя
        /// </summary>
        public ChangeTracker<Guid?> ParentId { get; }

        /// <summary>
        /// Ссылка на тип компонента
        /// </summary>
        public ChangeTracker<Guid> ComponentTypeId { get; }

        /// <summary>
        /// Версия компонента
        /// </summary>
        public ChangeTracker<string> Version { get; }

        /// <summary>
        /// Признак что компонент включен
        /// </summary>
        public ChangeTracker<bool> Enable { get; }

        /// <summary>
        /// Дата до которой выключен компонент
        /// </summary>
        public ChangeTracker<DateTime?> DisableToDate { get; }

        /// <summary>
        /// Комментарий к компоненту (указывается при выключении)
        /// </summary>
        public ChangeTracker<string> DisableComment { get; }

        /// <summary>
        /// Признак, что родитель включен
        /// </summary>
        public ChangeTracker<bool> ParentEnable { get; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public ChangeTracker<bool> IsDeleted { get; }

        #region Статусы

        /// <summary>
        /// Внутренний статус компонента.
        /// Учитываются проверки, метрики, события. 
        /// НЕ учитываются дочерние компоненты.
        /// </summary>
        public ChangeTracker<Guid> InternalStatusId { get; }

        /// <summary>
        /// Итоговый статус компонента.
        /// Учитываются проверки, метрики, события и дочерние компоненты (все учитывается)
        /// </summary>
        public ChangeTracker<Guid> ExternalStatusId { get; }

        /// <summary>
        /// Статус контрольного списка проверок
        /// </summary>
        public ChangeTracker<Guid> UnitTestsStatusId { get; }

        /// <summary>
        /// Статус всех пользовательских событий
        /// </summary>
        public ChangeTracker<Guid> EventsStatusId { get; }

        /// <summary>
        /// Статус всех метрик
        /// </summary>
        public ChangeTracker<Guid> MetricsStatusId { get; }

        /// <summary>
        /// Статус дочерних компонентов
        /// </summary>
        public ChangeTracker<Guid> ChildComponentsStatusId { get; }

        #endregion


    }
}