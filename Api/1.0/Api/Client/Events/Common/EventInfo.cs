using System;
using Zidium.Api.Common;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    /// <summary>
    /// Краткая информация о событии. 
    /// Используется в ответах методов по отправке событий
    /// </summary>
    public class EventInfo
    {
        public EventInfo(EventDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException("dto");
            }
            Id = dto.Id;
            OwnerId = dto.OwnerId;
            Importance = dto.Importance;
            Count = dto.Count;
            StartDate = dto.StartDate;
            EndDate = dto.EndDate;
            JoinKeyHash = dto.JoinKeyHash;
            TypeId = dto.TypeId;
            TypeSystemName = dto.TypeSystemName;
            TypeDisplayName = dto.TypeDisplayName;
            TypeCode = dto.TypeCode;
            Message = dto.Message;
            Version = dto.Version;
            Category = dto.Category;
            IsUserHandled = dto.IsUserHandled;
            Properties = DataConverter.GetExtentionPropertyCollection(dto.Properties);
        }

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

        public ExtentionPropertyCollection Properties { get; set; }
    }
}
