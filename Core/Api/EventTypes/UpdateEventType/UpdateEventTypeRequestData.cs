using System;
using Zidium.Api.Dto;
using Zidium.Storage;

namespace Zidium.Core.Api
{
    public class UpdateEventTypeRequestData
    {
        /// <summary>
        /// Ссылка на событие, по которому было принято решение изменить важность
        /// </summary>
        public Guid? EventId { get; set; }

        public Guid EventTypeId { get; set; }

        public string DisplayName { get; set; }

        public string SystemName { get; set; }

        public int? JoinIntervalSeconds { get; set; }

        public string OldVersion { get; set; }

        /// <summary>
        /// Переопределение важности для старых событий
        /// </summary>
        public EventImportance? ImportanceForOld { get; set; }
        
        /// <summary>
        /// Переопределение важности для новых событий
        /// </summary>
        public EventImportance? ImportanceForNew { get; set; }

        public bool UpdateActualEvents { get; set; }

    }
}
