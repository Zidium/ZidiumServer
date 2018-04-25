using System;

namespace Zidium.Api.Dto
{
    public class StateDataDto
    {
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime ActualDate { get; set; }

        public MonitoringStatus Status { get; set; }

        public string Message { get; set; }

        public bool HasSignal { get; set; }

        public string DisableComment { get; set; }

        public DateTime? DisableToDate { get; set; }
    }
}
