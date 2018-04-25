using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Models.Events
{
    public class ErrorStatisticsModel
    {
        public class EventTypeData
        {
            public EventType EventType { get; set; }

            public Event[] Events { get; set; }

            public OneLineSausageCharModel SausageModel { get; private set; }

            public void InitIntervals(DateTime fromTime, DateTime toTime)
            {
                var inervals = Events.Select(x => new OneLineSausageCharModel.IntervalData()
                {
                    ToTime = x.ActualDate,
                    FromTime = x.StartDate
                }).ToArray();
                var lineModel = new OneLineSausageCharModel()
                {
                    FromTime = fromTime,
                    ToTime = toTime,
                    NoDataColor = "#e7e7e7",
                    DataColor = GuiHelper.StrongRedBgColor
                };
                long count = Events.Sum(x => x.Count);
                lineModel.Init(inervals, count);
                SausageModel = lineModel;
            }
        }

        public enum SortOrder
        {
            ByCount = 0,
            ByDuration = 1
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

        public Guid? ComponentTypeId { get; set; }

        public Guid? ComponentId { get; set; }

        public Component Component { get; set; }

        public SimpleBarCharModel BarCharModel { get; set; }

        public List<EventTypeData> EventTypeDatas { get; set; }

        public SortOrder Order { get; set; }

        public ShowMode Mode { get; set; }

        public void LoadData(Guid accountId, AccountDbContext accountDbContext)
        {
            if (ComponentId.HasValue)
            {
                Component = accountDbContext.GetComponentRepository().GetById(ComponentId.Value);
            }

            var periodRange = ReportPeriodHelper.GetRange(Period);
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

            // создаем интервалы статистики
            var items = new List<SimpleBarCharModel.DataItem>();
            var date = FromTime;
            int itemIndex = 0;
            while (date < ToTime)
            {
                var item = new SimpleBarCharModel.DataItem()
                {
                    Index = itemIndex,
                    FromTime = date,
                    ToTime = date + TimeStep
                };
                date = date + TimeStep;
                itemIndex++;
                items.Add(item);
            }

            // загружаем события
            var events = accountDbContext.GetEventRepository().GetErrorsByPeriod(FromTime, ToTime);

            if (ComponentId.HasValue)
            {
                events = events.Where(x => x.OwnerId == ComponentId);
            }

            if (ComponentTypeId.HasValue)
            {
                var componentIdArray = accountDbContext.GetComponentRepository()
                        .QueryAll()
                        .Where(x => x.ComponentTypeId == ComponentTypeId.Value)
                        .Select(x => x.Id)
                        .ToArray();

                events = events.Where(x => componentIdArray.Contains(x.OwnerId));
            }

            if (Mode == ShowMode.NotProcessed)
            {
                events = events.Where(t => t.EventType.DefectId == null);
            }

            var eventsArray = events.OrderBy(x => x.StartDate).ToArray();

            // собираем статистику
            var intervals = new List<Event>[items.Count];
            foreach (var item in items)
            {
                intervals[item.Index] = new List<Event>();
            }
            foreach (var eventObj in eventsArray)
            {
                bool hasInterval = false;
                foreach (var timeInterval in items)
                {
                    // если интервал левее события
                    if (timeInterval.ToTime <= eventObj.StartDate)
                    {
                        continue;
                    }
                    // если интервал правее события
                    if (timeInterval.FromTime > eventObj.EndDate)
                    {
                        break;
                    }
                    hasInterval = true;
                    intervals[timeInterval.Index].Add(eventObj);
                }
                if (!hasInterval)
                {
                    MvcApplication.ComponentControl.AddApplicationError(
                        "Не удалось найти интервал статистики для события");
                }
            }
            foreach (var item in items)
            {
                var intervalEvents = intervals[item.Index];
                item.Value = intervalEvents.GroupBy(x => x.EventTypeId).Count();
            }
            BarCharModel = new SimpleBarCharModel(items.ToArray());

            // EventTypeDatas
            EventTypeDatas = new List<EventTypeData>();
            var eventTypeGroups = eventsArray.GroupBy(x => x.EventTypeId);
            foreach (var eventTypeGroup in eventTypeGroups)
            {
                Guid eventTypeId = eventTypeGroup.Key;
                var eventTypeData = new EventTypeData();
                var eventType = accountDbContext.EventTypes.Find(eventTypeId);
                if (eventType == null)
                {
                    MvcApplication.ComponentControl.AddApplicationError(
                        "Не удалось найти тип события " + eventTypeId);

                    continue;
                }
                eventTypeData.EventType = eventType;
                eventTypeData.Events = eventTypeGroup.ToArray();
                EventTypeDatas.Add(eventTypeData);
                eventTypeData.InitIntervals(FromTime, ToTime);
            }

            // сортируем типы
            if (Order == SortOrder.ByCount)
            {
                EventTypeDatas = EventTypeDatas.OrderByDescending(x => x.SausageModel.Count).ToList();
            }
            else if (Order == SortOrder.ByDuration)
            {
                EventTypeDatas = EventTypeDatas.OrderByDescending(x => x.SausageModel.Coverage).ToList();
            }
        }
    }
}