using System;
using Zidium.Core.Api;

namespace Zidium.Core.Common
{
    public class EventParameterTypeRow
    {
        public Guid Id { get; protected set; }

        public Guid EventTypeId { get; protected set; }

        public DataType DataType { get; protected set; }

        public string SystemName { get; protected set; }

        public string DisplayName { get; protected set; }

        public EventParameterTypeRow(
            Guid id,
            Guid eventTypeId,
            DataType dataType,
            string systemName,
            string displayName)
        {
            if (systemName == null)
            {
                throw new ArgumentNullException("systemName");
            }
            if (displayName == null)
            {
                throw new ArgumentNullException("displayName");
            }
            Id = id;
            EventTypeId = eventTypeId;
            DataType = dataType;
            SystemName = systemName;
            DisplayName = displayName;
        }
    }
}
