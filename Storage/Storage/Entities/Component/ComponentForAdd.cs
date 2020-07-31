using System;

namespace Zidium.Storage
{
    public class ComponentForAdd
    {
        public Guid Id;

        /// <summary>
        /// Дата и время создания компонента
        /// </summary>
        public DateTime CreatedDate;

        /// <summary>
        /// Отображаемое название компонента
        /// </summary>
        public string DisplayName;

        /// <summary>
        /// Внутреннее название компонента
        /// </summary>
        public string SystemName;

        /// <summary>
        /// Ссылка на родителя
        /// </summary>
        public Guid? ParentId;

        /// <summary>
        /// Ссылка на тип компонента
        /// </summary>
        public Guid ComponentTypeId;

        #region Статусы

        /// <summary>
        /// Внутренний статус компонента.
        /// Учитываются проверки, метрики, события. 
        /// НЕ учитываются дочерние компоненты.
        /// </summary>
        public Guid InternalStatusId;

        /// <summary>
        /// Итоговый статус компонента.
        /// Учитываются проверки, метрики, события и дочерние компоненты (все учитывается)
        /// </summary>
        public Guid ExternalStatusId;

        /// <summary>
        /// Статус контрольного списка проверок
        /// </summary>
        public Guid UnitTestsStatusId;

        /// <summary>
        /// Статус всех пользовательских событий
        /// </summary>
        public Guid EventsStatusId;

        /// <summary>
        /// Статус всех метрик
        /// </summary>
        public Guid MetricsStatusId;

        /// <summary>
        /// Статус дочерних компонентов
        /// </summary>
        public Guid ChildComponentsStatusId;

        #endregion

        /// <summary>
        /// Версия компонента
        /// </summary>
        public string Version;

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public bool IsDeleted;

        /// <summary>
        /// Признак что компонент включен
        /// </summary>
        public bool Enable;

        /// <summary>
        /// Дата до которой выключен компонент
        /// </summary>
        public DateTime? DisableToDate;

        /// <summary>
        /// Комментарий к компоненту (указывается при выключении)
        /// </summary>
        public string DisableComment;

        /// <summary>
        /// Признак, что родитель включен
        /// </summary>
        public bool ParentEnable;
    }
}
