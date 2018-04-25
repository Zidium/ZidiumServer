using System;

namespace Zidium.Api
{
    /// <summary>
    /// Базовый класс для отправки событий через IComponentControl
    /// </summary>
    public abstract class SendEventBase
    {
        protected SendEventBase(IComponentControl componentControl, string typeSystemName)
        {
            if (componentControl == null)
            {
                throw new ArgumentNullException("componentControl");
            }
            if (typeSystemName == null)
            {
                throw new ArgumentNullException("typeSystemName");
            }
            ComponentControl = componentControl;
            TypeSystemName = typeSystemName;
            Properties = new ExtentionPropertyCollection();
        }

        public IComponentControl ComponentControl { get; protected set; }

        public abstract SendEventCategory EventCategory { get; }

        /// <summary>
        /// Системное имя типа события
        /// </summary>
        public string TypeSystemName { get; internal set; }

        /// <summary>
        /// Отображаемое имя типа события
        /// </summary>
        public string TypeDisplayName { get; set; }

        /// <summary>
        /// Код события
        /// </summary>
        public string TypeCode { get; set; }

        /// <summary>
        /// Описание события
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Дата, когда событие произошло
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Если IsServerTime равно True, то StartDate не будет конвертироваться в серверное время (разница времени не учитывается).
        /// Если False (по умолчанию), то к StartDate будет прибавлено Client.TimeDifferenceSeconds (разница времени учитывается)
        /// </summary>
        public bool IsServerTime { get; set; }

        public int? Count { get; set; }

        /// <summary>
        /// Важность
        /// </summary>
        public EventImportance? Importance { get; set; }

        /// <summary>
        /// Ключ для склеивания событий
        /// </summary>
        public long? JoinKey { get; set; }

        /// <summary>
        /// Интервал объединения событий
        /// </summary>
        public TimeSpan? JoinInterval { get; set; }

        /// <summary>
        /// Версия компонента на момент создания события
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Если True - то событие не будет отправляться на сервер
        /// </summary>
        public bool Ignore { get; set; }

        /// <summary>
        /// Расширенные свойства
        /// </summary>
        public ExtentionPropertyCollection Properties { get; protected set; }

        public SendEventBase CreateBaseCopy()
        {
            var copy = MemberwiseClone();
            return copy as SendEventBase;
        }

        public SendEventResponse Send()
        {
            if (ComponentControl.IsFake())
            {
                return ResponseHelper.GetOfflineResponse<SendEventResponse>();
            }

            var client = (Client)ComponentControl.Client;
            return client.SendEventWrapper(this);
        }

        public AddEventResult Add()
        {
            if (ComponentControl.Client != null)
                return ComponentControl.Client.EventManager.AddEvent(this);

            return new AddEventResult(new BufferEventData(ComponentControl, this));
        }

    }
}
