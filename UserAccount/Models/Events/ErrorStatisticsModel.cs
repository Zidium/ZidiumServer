using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.Events
{
    public class ErrorStatisticsModel
    {
        public class EventTypeData
        {
            public EventTypeForRead EventType { get; set; }

            public int Count { get; set; }

            public string Message { get; set; }

            public Models.Defects.DefectControlModel DefectControl { get; set; }
        }

        public enum ShowMode
        {
            NotProcessed = 1,
            All = 2
        }

        public ReportPeriod Period { get; set; }

        public TimeSpan TimeStep { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public Guid? ComponentId { get; set; }

        public ComponentForRead Component { get; set; }

        public string Code { get; set; }

        public List<EventTypeData> EventTypeDatas { get; set; }

        public ShowMode Mode { get; set; }

        // TODO Move to controller
        public void LoadData(IStorage storage)
        {
            if (ComponentId.HasValue)
            {
                Component = storage.Components.GetOneById(ComponentId.Value);
            }

            var periodRange = ReportPeriodHelper.GetRange(Period, DateTime.UtcNow);
            ToTime = periodRange.To;
            FromTime = periodRange.From;

            // вычисляем интервалы
            if (Period == ReportPeriod.Hour)
            {
                TimeStep = TimeSpan.FromMinutes(5);
            }
            else if (Period == ReportPeriod.Day)
            {
                TimeStep = TimeSpan.FromHours(1);
            }
            else if (Period == ReportPeriod.Week)
            {
                TimeStep = TimeSpan.FromHours(4);
            }
            else if (Period == ReportPeriod.Month)
            {
                TimeStep = TimeSpan.FromDays(1);
            }
            else
            {
                throw new Exception("Неизвестное значение Period: " + Period);
            }

            // загружаем события
            var events = storage.Events.GetErrorsByPeriod(ComponentId, FromTime, ToTime, Mode == ShowMode.NotProcessed, Code);
            var eventTypes = storage.EventTypes.GetMany(events.Select(t => t.EventTypeId).Distinct().ToArray()).ToDictionary(a => a.Id, b => b);

            var data = events.Select(t => new
            {
                Event = t,
                EventType = eventTypes[t.EventTypeId]
            });

            var eventsArray = data.OrderBy(x => x.Event.StartDate).ToArray();

            // EventTypeDatas
            EventTypeDatas = new List<EventTypeData>();
            var eventTypeGroups = eventsArray.GroupBy(x => x.EventType);
            foreach (var eventTypeGroup in eventTypeGroups)
            {
                var eventTypeData = new EventTypeData
                {
                    EventType = eventTypeGroup.Key,
                    Count = eventTypeGroup.Sum(t => t.Event.Count),
                    Message = eventTypeGroup.First().Event.Message
                };

                var defect = eventTypeData.EventType.DefectId != null ? storage.Defects.GetOneById(eventTypeData.EventType.DefectId.Value) : null;
                var lastChange = defect != null ? storage.DefectChanges.GetLastByDefectId(defect.Id) : null;

                eventTypeData.DefectControl = new Defects.DefectControlModel()
                {
                    EventType = eventTypeData.EventType,
                    Defect = defect,
                    LastChange = lastChange
                };

                EventTypeDatas.Add(eventTypeData);
            }

            // сортируем типы
            EventTypeDatas = EventTypeDatas.OrderByDescending(x => x.Count).ToList();
        }
    }
}