﻿using System;
using Zidium.Api.Dto;
using Zidium.Core.Common.Helpers;
using Zidium.UserAccount.Helpers;

namespace Zidium.UserAccount.Models
{
    public class EventsListItemModel
    {
        public Guid Id { get; set; }

        public string Message { get; set; }

        public Guid OwnerId { get; set; }

        public EventCategory Category { get; set; }

        public Guid EventTypeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime ActualDate { get; set; }

        public EventImportance Importance { get; set; }

        public int Count { get; set; }

        public string Source { get; set; }

        public long JoinKey { get; set; }

        public TimeSpan Duration
        {
            get { return EventHelper.GetDuration(StartDate, ActualDate, DateTime.Now); }
        }

        public DateTime RealEndDate
        {
            get
            {
                return StartDate + Duration;
            }
        }

        public string RealEndDateString
        {
            get
            {
                return ActualDate == DateTimeHelper.InfiniteActualDate
                    ? null
                    : RealEndDate.ToString(GuiHelper.DateTimeDisplayFormat);
            }
        }

    }
}