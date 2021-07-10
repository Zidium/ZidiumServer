using System;
using Zidium.Api.Dto;

namespace Zidium.UserAccount.Models.ComponentTreeDetails
{
    public class UnittestDetailsHistoryModel
    {
        public Guid Id { get; set; }

        public StatusModel[] Statuses { get; set; }

        public class StatusModel
        {
            public Guid EventId { get; set; }

            public EventImportance Importance { get; set; }

            public DateTime StartDate { get; set; }

            public TimeSpan Duration { get; set; }

            public int Count { get; set; }

            public string Message { get; set; }

            public DateTime EndDate
            {
                get
                {
                    return StartDate + Duration;
                }
            }
        }
    }
}