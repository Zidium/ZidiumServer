using System;
using System.Collections.Generic;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    /// <summary>
    /// Компоненты
    /// </summary>
    public class Component
    {
        public Component()
        {
            Childs = new HashSet<Component>();
            Properties = new HashSet<ComponentProperty>();
            UnitTests = new HashSet<UnitTest>();
            Metrics = new HashSet<Metric>();
            LastNotifications = new HashSet<LastComponentNotification>();
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
        public virtual HashSet<Component> Childs { get; set; }

        /// <summary>
        /// Родительский компонент
        /// </summary>
        public virtual Component Parent { get; set; }

        /// <summary>
        /// Тип компонента
        /// </summary>
        public virtual ComponentType ComponentType { get; set; }

        public virtual LogConfig LogConfig { get; set; }

        /// <summary>
        /// Расширенные свойства
        /// </summary>
        public virtual HashSet<ComponentProperty> Properties { get; set; }

        public void SetProperty(ComponentProperty property)
        {
            var old = Properties.FirstOrDefault(x => x.Name == property.Name);
            if (old == null)
            {
                Properties.Add(property);
            }
            else
            {
                old.Value = property.Value;
                old.DataType = property.DataType;
            }
        }

        /// <summary>
        /// Юнит-тесты
        /// </summary>
        public virtual HashSet<UnitTest> UnitTests { get; set; }

        /// <summary>
        /// Метрики компонента
        /// </summary>
        public virtual HashSet<Metric> Metrics { get; set; }

        /// <summary>
        /// Последние уведомления о статусе компонента
        /// </summary>
        public virtual HashSet<LastComponentNotification> LastNotifications { get; set; }

        #region Колбаски

        public Guid InternalStatusId { get; set; }

        /// <summary>
        /// Внутренний статус компонента.
        /// Учитываются проверки, метрики, события. 
        /// НЕ учитываются дочерние компоненты.
        /// </summary>
        public virtual Bulb InternalStatus { get; set; }

        public Guid ExternalStatusId { get; set; }

        /// <summary>
        /// Итоговый статус компонента.
        /// Учитываются проверки, метрики, события и дочерние компоненты (все учитывается)
        /// </summary>
        public virtual Bulb ExternalStatus { get; set; }

        public Guid UnitTestsStatusId { get; set; }

        /// <summary>
        /// Статус контрольного списка проверок
        /// </summary>
        public virtual Bulb UnitTestsStatus { get; set; }

        public Guid EventsStatusId { get; set; }

        /// <summary>
        /// Статус всех пользовательских событий
        /// </summary>
        public virtual Bulb EventsStatus { get; set; }

        public Guid MetricsStatusId { get; set; }

        /// <summary>
        /// Статус всех метрик
        /// </summary>
        public virtual Bulb MetricsStatus { get; set; }

        public Guid ChildComponentsStatusId { get; set; }

        /// <summary>
        /// Статус дочерних компонентов
        /// </summary>
        public virtual Bulb ChildComponentsStatus { get; set; }

        #endregion

        /// <summary>
        /// Признак корневого компонента
        /// </summary>
        public bool IsRoot
        {
            get { return ComponentTypeId == SystemComponentTypes.Root.Id; }
        }

        /// <summary>
        /// Признак папки
        /// </summary>
        public bool IsFolder
        {
            get { return ComponentTypeId == SystemComponentTypes.Folder.Id; }
        }

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

        /// <summary>
        /// Компонент и его родитель включены
        /// </summary>
        public bool CanProcess
        {
            get
            {
                return Enable && ParentEnable;
            }
        }

        /*
        /// <summary>
        /// Возвращает все дочерние компоненты всех уровней
        /// </summary>
        /// <returns></returns>
        public List<Component> GetAllLevelChilds()
        {
            var result = Childs.Where(x => x.IsDeleted == false).ToList();
            foreach (var child in result)
            {
                var childs = child.GetAllLevelChilds();
                result.AddRange(childs);
            }
            return result;
        }
        */

        /*
        public int GetLevel()
        {
            int level = 0;
            var current = this;
            while (true)
            {
                if (current.ParentId == null)
                {
                    return level;
                }
                level++;
                current = current.Parent;
            }
        }
        */

        public List<Bulb> GetAllStatuses()
        {
            return new List<Bulb>()
            {
                ExternalStatus,
                InternalStatus,
                ChildComponentsStatus,
                UnitTestsStatus,
                MetricsStatus,
                EventsStatus
            };
        }

        public IQueryable<UnitTest> GetUnitTests()
        {
            return UnitTests.Where(t => t.IsDeleted == false).AsQueryable();
        }

        public IQueryable<Metric> GetMetrics()
        {
            return Metrics.Where(t => t.IsDeleted == false && t.MetricType.IsDeleted == false).AsQueryable();
        }

        public string GetFullDisplayName()
        {
            var list = new List<string>();
            var component = this;
            while (component != null)
            {
                list.Add(component.DisplayName);
                component = component.Parent;
            }
            list.Reverse();
            return string.Join(" / ", list);
        }
    }
}
