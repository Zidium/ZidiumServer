using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;

namespace Zidium.UserAccount.Models.Events
{
    public class ErrorStatisticsModel
    {
        public class EventTypeData
        {
            public EventType EventType { get; set; }

            public int Count { get; set; }

            public string Message { get; set; }
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

        public Component Component { get; set; }

        public List<EventTypeData> EventTypeDatas { get; set; }

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

            // загружаем события
            var events = accountDbContext.GetEventRepository().GetErrorsByPeriod(FromTime, ToTime).Include(t => t.EventType);

            if (ComponentId.HasValue)
            {
                events = events.Where(x => x.OwnerId == ComponentId);
            }

            if (Mode == ShowMode.NotProcessed)
            {
                events = events.Where(t => t.EventType.DefectId == null);
            }

            var eventsArray = events.OrderBy(x => x.StartDate).ToArray();

            // EventTypeDatas
            EventTypeDatas = new List<EventTypeData>();
            var eventTypeGroups = eventsArray.GroupBy(x => x.EventType);
            foreach (var eventTypeGroup in eventTypeGroups)
            {
                var eventTypeData = new EventTypeData
                {
                    EventType = eventTypeGroup.Key,
                    Count = eventTypeGroup.Sum(t => t.Count),
                    Message = eventTypeGroup.First().Message
                };
                EventTypeDatas.Add(eventTypeData);
            }

            // сортируем типы
            EventTypeDatas = EventTypeDatas.OrderByDescending(x => x.Count).ToList();

        }
    }
}