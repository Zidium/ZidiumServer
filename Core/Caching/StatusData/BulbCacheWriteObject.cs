using System;
using Zidium.Api.Others;
using Zidium.Storage;

namespace Zidium.Core.Caching
{
    public class BulbCacheWriteObject : CacheWriteObjectBase<AccountCacheRequest, BulbCacheResponse, IBulbCacheReadObject, BulbCacheWriteObject>, IBulbCacheReadObject
    {
        public override int GetCacheSize()
        {
            return 16 // Id
                   + 16 // ParentId
                   + 16 // ComponentId
                   + 16 // UnitTestId
                   + 16 // MetricId
                   + 16 // AccountId
                   + 4 // EventCategory
                   + 8 // CreateDate
                   + 4 // Status
                   + 8 // StartDate
                   + 8 // EndDate
                   + 4 // Count
                   + 8 // ActualDate
                   + StringHelper.GetPropertySize(Message)
                   + 16 // FirstEventId
                   + 16 // LastEventId
                   + 16 // LastChildStatusDataId
                   + 16 // StatusEventId
                   + 1 // HasSignal
                   + 1 // MoreImportance
                   + 1; //IsDeleted
        }

        public Guid? ComponentId { get; set; }

        public Guid? UnitTestId { get; set; }

        public Guid? MetricId { get; set; }

        public Guid AccountId { get; set; }

        public EventCategory EventCategory { get; private set; }

        public DateTime CreateDate { get; private set; }

        public MonitoringStatus Status { get; set; }

        public MonitoringStatus PreviousStatus { get; set; }

        /// <summary>
        /// Время начала статуса
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Фиксированное время завершения статуса, новые статусы могут начаться только позже данной даты
        /// </summary>
        public DateTime EndDate { get; set; }

        public int Count { get; set; }

        public DateTime ActualDate { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// Первое событие, которое привело к данному статусу
        /// </summary>
        public Guid? FirstEventId { get; set; }

        /// <summary>
        /// Последнее событие, которое привело к данному статусу 
        /// (нужно, чтобы пересчитывать статус, когда LastEvent меняет важность)
        /// </summary>
        public Guid? LastEventId { get; set; }

        /// <summary>
        /// Ссылка на последний дочерний компонент, который обновил данный статус
        /// </summary>
        public Guid? LastChildBulbId { get; set; }

        /// <summary>
        /// Ссылка на соответствующее событие статуса
        /// </summary>
        public Guid StatusEventId { get; set; }

        /// <summary>
        /// Длительность статуса
        /// </summary>
        public TimeSpan GetDuration(DateTime now)
        {
            if (ActualDate > now)
            {
                return now - StartDate;
            }
            return ActualDate - StartDate;
        }

        public bool HasSignal { get; set; }

        public bool IsSpace => !HasSignal;

        public bool IsDeleted { get; set; }

        public bool Actual(DateTime date)
        {
            return ActualDate >= date;
        }

        public Guid GetOwnerId()
        {
            if (ComponentId.HasValue)
            {
                return ComponentId.Value;
            }
            if (UnitTestId.HasValue)
            {
                return UnitTestId.Value;
            }
            if (MetricId.HasValue)
            {
                return MetricId.Value;
            }
            throw new Exception("Не удалось найти OwnerId");
        }

        /// <summary>
        /// Лист дерева статусов (НЕ имеет детей)
        /// </summary>
        public bool IsLeaf
        {
            get
            {
                return EventCategory == EventCategory.MetricStatus
                       || EventCategory == EventCategory.UnitTestStatus
                       || EventCategory == EventCategory.ComponentEventsStatus;
            }
        }

        public Guid? GetParentId()
        {
            var cache = new AccountCache(AccountId);
            if (EventCategory == EventCategory.UnitTestStatus)
            {
                if (UnitTestId == null)
                    throw new Exception($"EventCategory is UnitTestStatus, but UnitTestId is null, id: {Id}, account: {AccountId}");

                var unitTest = cache.UnitTests.Read(UnitTestId.Value);
                return cache.Components.Read(unitTest.ComponentId).UnitTestsStatusId;
                //if (UnitTest == null)
                //{
                //    return null;
                //}
                //return UnitTest.Component.UnitTestsStatus;
            }
            if (EventCategory == EventCategory.MetricStatus)
            {
                if (MetricId == null)
                    throw new Exception($"EventCategory is MetricStatus, but MetricId is null, id: {Id}, account: {AccountId}");

                var metric = cache.Metrics.Read(MetricId.Value);
                return cache.Components.Read(metric.ComponentId).MetricsStatusId;
                //if (Metric == null)
                //{
                //    return null;
                //}
                //return Metric.Component.MetricsStatus;
            }
            if (ComponentId == null)
            {
                return null;
            }
            var component = cache.Components.Read(ComponentId.Value);
            if (EventCategory == EventCategory.ComponentUnitTestsStatus)
            {
                return component.InternalStatusId;
            }
            
            if (EventCategory == EventCategory.ComponentMetricsStatus)
            {
                return component.InternalStatusId;
            }
            if (EventCategory == EventCategory.ComponentEventsStatus)
            {
                return component.InternalStatusId;
            }
            if (EventCategory == EventCategory.ComponentChildsStatus)
            {
                return component.ExternalStatusId;
            }
            if (EventCategory == EventCategory.ComponentInternalStatus)
            {
                return component.ExternalStatusId;
            }
            if (EventCategory == EventCategory.ComponentExternalStatus)
            {
                if (component.IsRoot || component.ParentId == null)
                {
                    return null;
                }
                var parent = cache.Components.Read(component.ParentId.Value);
                return parent.ChildComponentsStatusId;
                // return Component.Parent.ChildComponentsStatus;
            }
            throw new Exception("Неизвестное значение EventCategory: " + EventCategory);
        }

        public BulbForUpdate CreateEfBulb()
        {
            var result = new BulbForUpdate(Id);

            return result;
        }

        public static BulbCacheWriteObject Create(BulbForRead data, Guid accountId)
        {
            if (data == null)
            {
                return null;
            }
            var result = new BulbCacheWriteObject()
            {
                Id = data.Id,
                CreateDate = data.CreateDate,
                AccountId = accountId,
                IsDeleted = data.IsDeleted,
                LastChildBulbId = data.LastChildBulbId,
                ActualDate = data.ActualDate,
                Status = data.Status,
                Count = data.Count,
                StartDate = data.StartDate,
                Message = data.Message,
                StatusEventId = data.StatusEventId,
                UnitTestId = data.UnitTestId,
                PreviousStatus = data.PreviousStatus,
                MetricId = data.MetricId,
                LastEventId = data.LastEventId,
                HasSignal = data.HasSignal,
                FirstEventId = data.FirstEventId,
                EventCategory = data.EventCategory,
                EndDate = data.EndDate,
                ComponentId = data.ComponentId
            };

            return result;
        }
    }
}
