using System;
using System.Linq;
using Zidium.Api.Others;
using Zidium.Core.AccountsDb;
using Zidium.Storage;

namespace Zidium.Core.Caching
{
    public class ComponentCacheWriteObject : CacheWriteObjectBase<AccountCacheRequest, ComponentCacheResponse, IComponentCacheReadObject, ComponentCacheWriteObject>, IComponentCacheReadObject
    {
        public ComponentCacheWriteObject()
        {
            WriteChilds = new CacheObjectReferenceCollection();
            WriteUnitTests = new CacheObjectReferenceCollection();
            WriteMetrics = new CacheObjectReferenceCollection();
        }

        public override ComponentCacheWriteObject GetCopy()
        {
            var copy = base.GetCopy();
            copy.WriteChilds = WriteChilds.GetCopy();
            copy.WriteUnitTests = WriteUnitTests.GetCopy();
            copy.WriteMetrics = WriteMetrics.GetCopy();
            return copy;
        }

        public override int GetCacheSize()
        {
            return 16 // id
                   + 8 // CreateDate
                   + StringHelper.GetPropertySize(DisplayName)
                   + StringHelper.GetPropertySize(SystemName)
                   + 16 // parentId
                   + 16 // ComponentTypeId
                   + 16 // AccountId
                   + 300; // лень считать точно
        }

        /// <summary>
        /// Номер версии ВСЕХ ссылок
        /// </summary>
        public int ReferencesVersion
        {
            get { return WriteChilds.Version + WriteMetrics.Version + WriteUnitTests.Version; }
        }

        /// <summary>
        /// Дата и время создания компонента
        /// </summary>
        public DateTime CreatedDate { get; private set; }

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
        /// Ссылка на аккаунт
        /// </summary>
        public Guid AccountId { get; set; }

        public IReadOnlyReferenceCollection Childs
        {
            get { return WriteChilds; }
        }

        public IReadOnlyReferenceCollection UnitTests
        {
            get { return WriteUnitTests; }
        }

        public IReadOnlyReferenceCollection Metrics
        {
            get { return WriteMetrics; }
        }

        /// <summary>
        /// Коллекция дочерних компонентов
        /// </summary>
        public CacheObjectReferenceCollection WriteChilds { get; protected set; }

        /// <summary>
        /// Юнит-тесты
        /// </summary>
        public CacheObjectReferenceCollection WriteUnitTests { get; protected set; }

        /// <summary>
        /// Метрики компонента
        /// </summary>
        public CacheObjectReferenceCollection WriteMetrics { get; protected set; }


        #region Колбаски

        public Guid InternalStatusId { get; set; }

        public Guid ExternalStatusId { get; set; }

        public Guid UnitTestsStatusId { get; set; }

        public Guid EventsStatusId { get; set; }

        public Guid MetricsStatusId { get; set; }
        
        public Guid ChildComponentsStatusId { get; set; }
        
        #endregion


        /// <summary>
        /// Признак корневого компонента
        /// </summary>
        public bool IsRoot
        {
            get { return ComponentTypeId == SystemComponentType.Root.Id; }
        }

        /// <summary>
        /// Признак папки
        /// </summary>
        public bool IsFolder
        {
            get { return ComponentTypeId == SystemComponentType.Folder.Id; }
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

        public ComponentForUpdate CreateEf()
        {
            return new ComponentForUpdate(Id);
        }

        public Guid[] GetAllStatusesIds()
        {
            return new []
            {
                ExternalStatusId,
                InternalStatusId,
                ChildComponentsStatusId,
                EventsStatusId,
                MetricsStatusId,
                UnitTestsStatusId
            };
        }

        public static ComponentCacheWriteObject Create(ComponentForRead component, Guid accountId, IStorage storage)
        {
            if (component == null)
            {
                return null;
            }
            var cache = new ComponentCacheWriteObject()
            {
                Id = component.Id,
                AccountId = accountId,
                IsDeleted = component.IsDeleted,
                InternalStatusId = component.InternalStatusId,
                ParentId = component.ParentId,
                ExternalStatusId = component.ExternalStatusId,
                EventsStatusId = component.EventsStatusId,
                Enable = component.Enable,
                DisplayName = component.DisplayName,
                DisableToDate = component.DisableToDate,
                DisableComment = component.DisableComment,
                CreatedDate = component.CreatedDate,
                ComponentTypeId = component.ComponentTypeId,
                ParentEnable = component.ParentEnable,
                SystemName = component.SystemName,
                MetricsStatusId = component.MetricsStatusId,
                UnitTestsStatusId = component.UnitTestsStatusId,
                Version = component.Version,
                ChildComponentsStatusId = component.ChildComponentsStatusId
            };

            // обновим ParentEnable
            if (component.ParentId != null)
            {
                var parent = storage.Components.GetOneById(component.ParentId.Value);
                cache.ParentEnable = parent.CanProcess();
            }

            // загрузим детей
            var childs = storage.Components.GetChilds(component.Id)
                .Select(x => new CacheObjectReference(x.Id, x.SystemName))
                .ToArray();

            cache.WriteChilds.AddRange(childs);

            // загрузим метрики
            var metrics = storage.Metrics.GetByComponentId(component.Id)
                .Select(x => new CacheObjectReference(x.Id, x.MetricTypeId.ToString())) // могут быть проблемы из-за того, что в кэше и БД разное имя типа метрики
                .ToArray();

            cache.WriteMetrics.AddRange(metrics);

            // загрузим юнит-тесты
            var unitTests = storage.UnitTests.GetByComponentId(component.Id)
                .Select(x => new CacheObjectReference(x.Id, x.SystemName))
                .ToArray();

            cache.WriteUnitTests.AddRange(unitTests);

            return cache;
        }
    }
}
