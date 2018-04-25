using System;
using Zidium.Api.Others;

namespace Zidium.Api
{
    public class BufferEventData
    {
        public BufferEventData(IComponentControl componentControl, SendEventBase eventBase)
        {
            if (componentControl == null)
            {
                throw new ArgumentNullException("componentControl");
            }
            if (eventBase == null)
            {
                throw new ArgumentNullException("eventBase");
            }
            ComponentControl = componentControl;
            SendEventBase = eventBase;
            Init();
        }

        public object SynchRoot
        {
            get { return this; }
        }

        public int Errors { get; set; }

        protected void Init()
        {
            CreateDate = DateTime.Now;
            LastAddDate = CreateDate;
            Status = AddEventStatus.WaitForSend;
        }

        public IComponentControl ComponentControl { get; protected set; }

        public long GlobalJoinKey { get; set; }

        public AddEventStatus Status { get; set; }

        /// <summary>
        /// Время создания события в очереди
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Время последнего добавления события в очередь
        /// </summary>
        public DateTime LastAddDate { get; set; }

        /// <summary>
        /// Время когда событие последний раз было успешно отправлено или склеено
        /// </summary>
        public DateTime? LastSuccessSendOrJoinDate { get; set; }

        /// <summary>
        /// Время когда событие последний раз пробовали отравлять или склеивать
        /// </summary>
        public DateTime? LastAttempSendOrJoinDate { get; set; }

        public Guid? EventId { get; set; }

        public Guid? EventTypeId { get; set; }

        public SendEventBase SendEventBase { get; protected set; }

        public ApplicationErrorData GetApplicationErrorData()
        {
            return SendEventBase as ApplicationErrorData;
        }

        public ComponentEventData GetComponentEvent()
        {
            return SendEventBase as ComponentEventData;
        }

        public int Size { get; set; }

        public int GetSize()
        {
            var size = IntPtr.Size * 4 +
                sizeof(int) * 8 +
                sizeof(long) * 2 +
                16 * 2 + // Guid
                8 * 6 + // DateTime
                8 * 2 // TimeSpan
                ; // поля с датами, важностью и прочее
            size += StringHelper.GetLengthInMemory(SendEventBase.TypeSystemName);
            size += StringHelper.GetLengthInMemory(SendEventBase.TypeDisplayName);
            size += StringHelper.GetLengthInMemory(SendEventBase.TypeCode);
            size += StringHelper.GetLengthInMemory(SendEventBase.Message);
            size += StringHelper.GetLengthInMemory(SendEventBase.Version);
            if (SendEventBase.Properties != null)
            {
                size += SendEventBase.Properties.GetInternalSize();
            }
            return size;
        }
    }
}
