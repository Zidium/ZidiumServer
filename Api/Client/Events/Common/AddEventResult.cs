using System;

namespace Zidium.Api
{
    public class AddEventResult
    {
        protected BufferEventData BufferEventData { get; set; }

        public AddEventResult(BufferEventData bufferEventData)
        {
            if (bufferEventData == null)
            {
                throw new ArgumentNullException("bufferEventData");
            }
            BufferEventData = bufferEventData;
        }

        public AddEventStatus Status
        {
            get { return BufferEventData.Status; }
        }

        public int Count
        {
            get { return BufferEventData.SendEventBase.Count ?? 1; }
        }

        public Guid? EventId
        {
             get { return BufferEventData.EventId; }
        }

        public int Errors
        {
            get { return BufferEventData.Errors; }
        }
    }
}
