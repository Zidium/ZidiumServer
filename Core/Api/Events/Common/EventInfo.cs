using System;
using System.Collections.Generic;

namespace Zidium.Core.Api
{
    /// <summary>
    /// Краткая информация о событии. 
    /// Используется в ответах методов по отправке событий
    /// </summary>
    public class EventInfo
    {
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public EventImportance Importance { get; set; }

        public int Count { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public long JoinKeyHash { get; set; }

        public Guid TypeId { get; set; }

        public string TypeSystemName { get; set; }

        public string TypeDisplayName { get; set; }

        public string TypeCode { get; set; }

        public string Message { get; set; }

        public string Version { get; set; }

        public EventCategory Category { get; set; }

        public bool IsUserHandled { get; set; }

        public Guid? LastStatusEventId { get; set; }

        public List<ExtentionPropertyDto> Properties { get; set; }
    }
}
