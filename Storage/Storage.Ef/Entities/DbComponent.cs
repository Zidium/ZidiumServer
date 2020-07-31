using System;
using System.Collections.Generic;

namespace Zidium.Storage.Ef
{
    /// <summary>
    /// Компоненты
    /// </summary>
    public class DbComponent
    {
        public DbComponent()
        {
            Childs = new HashSet<DbComponent>();
            Properties = new HashSet<DbComponentProperty>();
            UnitTests = new HashSet<DbUnitTest>();
            Metrics = new HashSet<DbMetric>();
            LastNotifications = new HashSet<DbLastComponentNotification>();
        }

        public Guid Id { get; set; }

        /// <summary>
        /// Дата и время создания компонента
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Отображаемое название компонента
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Внутреннее название компонента
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Ссылка на родителя
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Ссылка на тип компонента
        /// </summary>
        public Guid ComponentTypeId { get; set; }

        /// <summary>
        /// Коллекция дочерних компонентов
        /// </summary>
        public virtual HashSet<DbComponent> Childs { get; set; }

        /// <summary>
        /// Родительский компонент
        /// </summary>
        public virtual DbComponent Parent { get; set; }

        /// <summary>
        /// Тип компонента
        /// </summary>
        public virtual DbComponentType ComponentType { get; set; }

        public virtual DbLogConfig LogConfig { get; set; }

        /// <summary>
        /// Расширенные свойства
        /// </summary>
        public virtual HashSet<DbComponentProperty> Properties { get; set; }

        /// <summary>
        /// Юнит-тесты
        /// </summary>
        public virtual HashSet<DbUnitTest> UnitTests { get; set; }

        /// <summary>
        /// Метрики компонента
        /// </summary>
        public virtual HashSet<DbMetric> Metrics { get; set; }

        /// <summary>
        /// Последние уведомления о статусе компонента
        /// </summary>
        public virtual HashSet<DbLastComponentNotification> LastNotifications { get; set; }

        public Guid InternalStatusId { get; set; }

        /// <summary>
        /// Внутренний статус компонента.
        /// Учитываются проверки, метрики, события. 
        /// НЕ учитываются дочерние компоненты.
        /// </summary>
        public virtual DbBulb InternalStatus { get; set; }

        public Guid ExternalStatusId { get; set; }

        /// <summary>
        /// Итоговый статус компонента.
        /// Учитываются проверки, метрики, события и дочерние компоненты (все учитывается)
        /// </summary>
        public virtual DbBulb ExternalStatus { get; set; }

        public Guid UnitTestsStatusId { get; set; }

        /// <summary>
        /// Статус контрольного списка проверок
        /// </summary>
        public virtual DbBulb UnitTestsStatus { get; set; }

        public Guid EventsStatusId { get; set; }

        /// <summary>
        /// Статус всех пользовательских событий
        /// </summary>
        public virtual DbBulb EventsStatus { get; set; }

        public Guid MetricsStatusId { get; set; }

        /// <summary>
        /// Статус всех метрик
        /// </summary>
        public virtual DbBulb MetricsStatus { get; set; }

        public Guid ChildComponentsStatusId { get; set; }

        /// <summary>
        /// Статус дочерних компонентов
        /// </summary>
        public virtual DbBulb ChildComponentsStatus { get; set; }

        /// <summary>
        /// Версия компонента
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Признак что компонент включен
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// Дата до которой выключен компонент
        /// </summary>
        public DateTime? DisableToDate { get; set; }

        /// <summary>
        /// Комментарий к компоненту (указывается при выключении)
        /// </summary>
        public string DisableComment { get; set; }

        /// <summary>
        /// Признак, что родитель включен
        /// </summary>
        public bool ParentEnable { get; set; }

    }
}
