using System;
using Zidium.Api.Dto;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class ShowStatusesModel
    {
        public Guid UnitTestId { get; set; }

        public int MaxCount { get; set; }

        public UnitTestStatusEvent[] Statuses { get; set; }

        public class UnitTestStatusEvent
        {
            public Guid Id { get; set; }

            public Guid UnitTestId { get; set; }

            public DateTime Date { get; set; }

            public TimeSpan Duration { get; set; }

            public DateTime EndDate { get; set; }

            public int Count { get; set; }

            public string Message { get; set; }

            public EventImportance Importance { get; set; }
        }

    }
}