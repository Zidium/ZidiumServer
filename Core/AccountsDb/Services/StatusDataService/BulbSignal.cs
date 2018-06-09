using System;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Core.Common.Helpers;

namespace Zidium.Core.AccountsDb
{
    public class BulbSignal
    {
        public BulbSignal()
        {
        }

        public MonitoringStatus Status { get; set; }

        public string Message { get; set; }

        public DateTime ProcessDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime ActualDate { get; set; }

        /// <summary>
        /// Ссылка на событие-причину сигнала
        /// </summary>
        public Guid EventId { get; set; }

        public Guid AccountId { get; set; }

        public bool IsSpace { get; set; }

        public bool HasSignal => !IsSpace;

        public EventImportance NoSignalImportance { get; set; }

        public Guid? ChildBulbId { get; set; }

        public static BulbSignal CreateUnknown(Guid accountId, DateTime startDate)
        {
            var signal = new BulbSignal()
            {
                AccountId = accountId,
                StartDate = startDate,
                ProcessDate = startDate,
                ActualDate = DateTimeHelper.InfiniteActualDate,
                Message = "Нет данных",
                Status = MonitoringStatus.Unknown,
                NoSignalImportance = EventImportance.Unknown,
                IsSpace = true // todo ??? пробел ли это ???
            };
            return signal;
        }

        public static BulbSignal CreateNoSignal(Guid accountId, DateTime startDate, EventImportance noSignalImportance)
        {
            var noSignalStatus = MonitoringStatusHelper.Get(noSignalImportance);
            var signal = new BulbSignal()
            {
                AccountId = accountId,
                StartDate = startDate,
                ProcessDate = startDate,
                ActualDate = DateTimeHelper.InfiniteActualDate,
                Message = "Нет сигнала",
                Status = noSignalStatus,
                NoSignalImportance = EventImportance.Unknown,
                IsSpace = true
            };
            return signal;
        }

        public static BulbSignal CreateDisable(Guid accountId, DateTime processTime)
        {
            var signal = new BulbSignal()
            {
                AccountId = accountId,
                ActualDate = DateTimeHelper.InfiniteActualDate, // чтобы статус был актуален вечно
                StartDate = processTime,
                ProcessDate = processTime,
                Status = MonitoringStatus.Disabled,
                Message = "Объект выключен",
                NoSignalImportance = EventImportance.Unknown // при выключении пробел будет серым
            };
            return signal;
        }

        public static BulbSignal CreateFromChild(DateTime processDate, IBulbCacheReadObject child)
        {
            var signal = new BulbSignal()
            {
                AccountId = child.AccountId,
                Status = child.Status,
                NoSignalImportance = EventImportance.Unknown,
                StartDate = processDate,
                ProcessDate = processDate,
                ActualDate = DateTimeHelper.InfiniteActualDate,
                EventId = child.StatusEventId,
                IsSpace = child.IsSpace,
                Message = child.Message,
                ChildBulbId = child.Id
            };
            return signal;
        }

        public static BulbSignal Create(DateTime processDate, IEventCacheReadObject eventObj, EventImportance noSignalImportance)
        {
            if (eventObj == null)
            {
                throw new ArgumentNullException("eventObj");
            }
            return new BulbSignal()
            {
                AccountId = eventObj.AccountId,
                EventId = eventObj.Id,
                IsSpace = eventObj.IsSpace,
                ActualDate = eventObj.ActualDate,
                Message = eventObj.Message,
                Status = MonitoringStatusHelper.Get(eventObj.Importance),
                NoSignalImportance = noSignalImportance,
                ProcessDate = processDate,
                StartDate = eventObj.StartDate
            };
        }

        public static BulbSignal Create(DateTime processDate, Event eventObj, EventImportance noSignalImportance, Guid accountId)
        {
            if (eventObj == null)
            {
                throw new ArgumentNullException("eventObj");
            }
            return new BulbSignal()
            {
                AccountId = accountId,
                EventId = eventObj.Id,
                IsSpace = eventObj.IsSpace,
                ActualDate = eventObj.ActualDate,
                Message = eventObj.Message,
                Status = MonitoringStatusHelper.Get(eventObj.Importance),
                NoSignalImportance = noSignalImportance,
                ProcessDate = processDate,
                StartDate = eventObj.StartDate
            };
        }
    }
}
