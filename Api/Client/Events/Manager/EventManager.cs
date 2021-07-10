using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Zidium.Api.Dto;
using Zidium.Api.Others;

namespace Zidium.Api
{
    public class EventManager : IEventManager
    {
        protected Timer ProcessEventsTimer { get; set; }

        protected Client Client { get; set; }

        public IEventQueue Queue { get; protected set; }

        protected ThreadTaskQueue TaskQueue { get; set; }

        protected PrepareDataHelper PrepareDataHelper { get; set; }

        public bool Disabled
        {
            get { return Client.Config.Events.EventManager.Disabled; }
            set { Client.Config.Events.EventManager.Disabled = value; }
        }

        public EventManager(Client client)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }
            Client = client;
            Queue = new EventQueue(client);
            TaskQueue = new ThreadTaskQueue(client.Config.Events.EventManager.Threads);
            PrepareDataHelper = new PrepareDataHelper(client);
        }

        protected long GetGlobalJoinKey(IComponentControl componentControl, SendEventBase eventData)
        {
            return HashHelper.GetInt64(
                componentControl.SystemName,
                eventData.TypeSystemName,
                eventData.JoinKey.ToString(),
                eventData.EventCategory.ToString(),
                (eventData.Importance ?? EventImportance.Unknown).ToString(),
                eventData.Version);
        }

        protected virtual void PrepareBufferEventData(BufferEventData bufferEvent)
        {
            // обрежим длинные свойства события
            PrepareDataHelper.PrepareEvent(bufferEvent.SendEventBase);

            // установим GlobalJoinKey
            bufferEvent.GlobalJoinKey = GetGlobalJoinKey(
                bufferEvent.ComponentControl,
                bufferEvent.SendEventBase);
        }

        protected virtual bool AreJoinPropertiesEqual(BufferEventData a, BufferEventData b)
        {
            // должны быть равны = EventCategory + JoinKey + IsServerTime + ComponentSystemName + TypeSystemName
            return a.SendEventBase.EventCategory == b.SendEventBase.EventCategory
                && a.SendEventBase.JoinKey == b.SendEventBase.JoinKey
                && (a.SendEventBase.Importance ?? EventImportance.Unknown) == (b.SendEventBase.Importance ?? EventImportance.Unknown)
                && a.SendEventBase.IsServerTime == b.SendEventBase.IsServerTime
                && string.Equals(
                    a.ComponentControl.SystemName,
                    b.ComponentControl.SystemName,
                    StringComparison.OrdinalIgnoreCase)
                && string.Equals(
                    a.SendEventBase.TypeSystemName,
                    b.SendEventBase.TypeSystemName,
                    StringComparison.OrdinalIgnoreCase);
        }

        protected int AddCount(int value, int add)
        {
            long result = (long)value + add;
            if (result > Int32.MaxValue)
            {
                return Int32.MaxValue;
            }
            return (int)result;
        }

        protected BufferEventData AddOrJoin(BufferEventData newBufferEvent)
        {
            Queue.MaxSizeBytes = Client.Config.Events.EventManager.QueueBytes;
            PrepareBufferEventData(newBufferEvent);
            lock (Queue.SynchRoot)
            {
                // если JoinInterval==0, то склеивать нельзя
                if (newBufferEvent.SendEventBase.JoinInterval == TimeSpan.Zero)
                {
                    Queue.Add(newBufferEvent);
                    return newBufferEvent;
                }

                // проверим, можно ли склеить
                var bufferEvents = Queue.GetAllByGlobalJoinKey(newBufferEvent.GlobalJoinKey);

                // склеивать можно только с последним, поэтому отсортируем по дате создания
                bufferEvents = bufferEvents.OrderByDescending(x => x.CreateDate).ToList();

                var lastEvent = bufferEvents.FirstOrDefault(x => AreJoinPropertiesEqual(x, newBufferEvent));

                // если это первое событие
                if (lastEvent == null)
                {
                    Queue.Add(newBufferEvent);
                    return newBufferEvent;
                }

                // рассчитаем время склейки
                TimeSpan joinInterval = newBufferEvent.SendEventBase.JoinInterval ?? TimeSpan.Zero;
                var currentDate = newBufferEvent.SendEventBase.StartDate.Value;
                bool canJoinByTime = currentDate + joinInterval >= currentDate;

                // если событие уже было и время склейки НЕ протухло
                if (canJoinByTime)
                {
                    lock (lastEvent.SynchRoot)
                    {
                        // увеличим счетчик
                        lastEvent.SendEventBase.Count = AddCount(lastEvent.SendEventBase.Count ?? 1, newBufferEvent.SendEventBase.Count ?? 1);

                        // дату начала не меняем!

                        // обновим сообщение
                        lastEvent.SendEventBase.Message = newBufferEvent.SendEventBase.Message;

                        // обновим важность
                        lastEvent.SendEventBase.Importance = newBufferEvent.SendEventBase.Importance;

                        lastEvent.LastAddDate = newBufferEvent.CreateDate;

                        if (lastEvent.Status == AddEventStatus.Sended)
                        {
                            lastEvent.Status = AddEventStatus.WaitForJoin;
                        }
                        else if (lastEvent.Status == AddEventStatus.Joined)
                        {
                            lastEvent.Status = AddEventStatus.WaitForJoin;
                        }
                    }
                    return lastEvent;
                }

                // если такое событие уже было, но время склейки протухло
                Queue.Add(newBufferEvent);
                return newBufferEvent;
            }
        }

        public AddEventResult AddEvent(SendEventBase eventBase)
        {
            var inputEvent = new BufferEventData(eventBase.ComponentControl, eventBase);
            if (eventBase.Ignore)
            {
                // если игнорируем, то в очередь НЕ помещем
                inputEvent.Status = AddEventStatus.Removed;
                return new AddEventResult(inputEvent);
            }
            var outputEvent = AddOrJoin(inputEvent);
            return new AddEventResult(outputEvent);
        }

        protected void SendOneEvent(BufferEventData bufferEventData)
        {
            if (Client.CanSendData == false)
            {
                return;
            }
            if (Client.CanConvertToServerDate() == false)
            {
                return;
            }
            bool success = false;
            try
            {
                int oldCount = 0;

                SendEventBase eventBase = null;

                lock (bufferEventData.SynchRoot)
                {
                    oldCount = bufferEventData.SendEventBase.Count.Value;
                    if (bufferEventData.Status != AddEventStatus.WaitForSend)
                    {
                        success = true;
                        return;
                    }
                    eventBase = bufferEventData.SendEventBase.CreateBaseCopy();
                    bufferEventData.Status = AddEventStatus.BeginSend;
                }

                var response = eventBase.Send();

                lock (bufferEventData.SynchRoot)
                {
                    bufferEventData.LastAttempSendOrJoinDate = DateTime.Now;
                    if (response.Success)
                    {
                        bufferEventData.EventId = response.GetDataAndCheck().EventId;
                        bufferEventData.EventTypeId = response.GetDataAndCheck().EventTypeId;
                        bufferEventData.Errors = 0;
                        bufferEventData.LastSuccessSendOrJoinDate = bufferEventData.LastAttempSendOrJoinDate;

                        // вычтим из текущего Count то что отправили
                        if (bufferEventData.SendEventBase.Count < int.MaxValue)
                        {
                            bufferEventData.SendEventBase.Count -= oldCount;
                        }
                        if (bufferEventData.SendEventBase.Count < 0)
                        {
                            bufferEventData.SendEventBase.Count = 0;
                        }

                        if (bufferEventData.Status != AddEventStatus.Removed)
                        {
                            if (bufferEventData.SendEventBase.Count > 1)
                            {
                                bufferEventData.Status = AddEventStatus.WaitForJoin;
                            }
                            else
                            {
                                bufferEventData.Status = AddEventStatus.Sended;
                            }
                        }
                        success = true;
                    }
                }
            }
            catch (Exception exception)
            {
                Client.InternalLog.Error("Ошибка отправки события из очереди", exception);
            }
            finally
            {
                lock (bufferEventData.SynchRoot)
                {
                    if (success)
                    {

                    }
                    else
                    {
                        if (bufferEventData.Status != AddEventStatus.Removed)
                        {
                            bufferEventData.Status = AddEventStatus.WaitForSend;
                        }
                        bufferEventData.Errors++;
                    }
                }
            }
        }

        protected class JoinEventTempClass
        {
            public BufferEventData BufferEvent { get; set; }

            public JoinEventRequestDataDto JoinEvent { get; set; }
        }

        protected void JoinEvents(List<BufferEventData> events)
        {
            if (Client.CanSendData == false)
            {
                return;
            }
            bool success = false;
            var joinEvents = new List<JoinEventTempClass>(events.Count);
            try
            {
                foreach (var bufferEventData in events)
                {
                    lock (bufferEventData.SynchRoot)
                    {
                        if (bufferEventData.Status == AddEventStatus.WaitForJoin)
                        {
                            var joinData = new JoinEventRequestDataDto()
                            {
                                EventId = bufferEventData.EventId,
                                ComponentId = bufferEventData.ComponentControl.Info.Id,
                                Count = bufferEventData.SendEventBase.Count,
                                Importance = bufferEventData.SendEventBase.Importance,
                                StartDate = bufferEventData.SendEventBase.StartDate,
                                TypeId = bufferEventData.EventTypeId,
                                JoinKey = bufferEventData.SendEventBase.JoinKey,
                                JoinInterval = bufferEventData.SendEventBase.JoinInterval?.TotalSeconds,
                                Message = bufferEventData.SendEventBase.Message,
                                Version = bufferEventData.SendEventBase.Version
                            };
                            joinEvents.Add(new JoinEventTempClass()
                            {
                                BufferEvent = bufferEventData,
                                JoinEvent = joinData
                            });
                            bufferEventData.Status = AddEventStatus.BeginJoin;
                        }
                    }
                }
                var joinDatas = joinEvents.Select(x => x.JoinEvent).ToList();
                var response = Client.ApiService.JoinEvents(joinDatas);
                success = response.Success;
            }
            catch (Exception exception)
            {
                Client.InternalLog.Error("Ошибка склейки событий из очереди", exception);
            }
            finally
            {
                var now = DateTime.Now;
                foreach (var joinEvent in joinEvents)
                {
                    var bufferEvent = joinEvent.BufferEvent;
                    lock (bufferEvent.SynchRoot)
                    {
                        bufferEvent.LastAttempSendOrJoinDate = now;
                        if (success)
                        {
                            bufferEvent.LastSuccessSendOrJoinDate = now;
                            bufferEvent.Errors = 0;

                            bufferEvent.SendEventBase.Count -= joinEvent.JoinEvent.Count;
                            if (bufferEvent.SendEventBase.Count < 0)
                            {
                                bufferEvent.SendEventBase.Count = 0;
                            }

                            if (bufferEvent.Status != AddEventStatus.Removed)
                            {
                                if (bufferEvent.SendEventBase.Count == 0)
                                {
                                    bufferEvent.Status = AddEventStatus.Joined;
                                }
                                else
                                {
                                    bufferEvent.Status = AddEventStatus.WaitForJoin;
                                }
                            }
                        }
                        else
                        {
                            bufferEvent.Errors++;
                            if (bufferEvent.Status != AddEventStatus.Removed)
                            {
                                bufferEvent.Status = AddEventStatus.WaitForJoin;
                            }
                        }
                    }
                }
            }
        }

        protected virtual void BeginFlush(DateTime date)
        {
            try
            {
                if (Disabled)
                {
                    return;
                }
                if (Queue.Count() == 0)
                {
                    return;
                }

                if (Client.CanSendData == false)
                {
                    return;
                }

                if (Client.CanConvertToServerDate() == false)
                {
                    return;
                }

                List<BufferEventData> allEvents = null;
                lock (Queue.SynchRoot)
                {
                    allEvents = Queue.GetAll();
                }

                if (allEvents == null || allEvents.Count == 0)
                {
                    return;
                }

                // получаем события для отправки
                int maxSendCount = Client.Config.Events.EventManager.MaxSend;

                var sendEvents = allEvents
                    .Where(x =>
                        x.Status == AddEventStatus.WaitForSend
                        && x.CreateDate <= date
                        && x.ComponentControl.IsFake() == false)
                    .OrderBy(x => x.Errors)
                    .ThenBy(x => x.CreateDate)
                    .Take(maxSendCount)
                    .ToList();

                // получаем события для склейки
                int maxJoinCount = Client.Config.Events.EventManager.MaxJoin;

                var joinEvents = allEvents
                    .Where(x =>
                        x.Status == AddEventStatus.WaitForJoin
                        && x.ComponentControl.IsFake() == false
                        && x.LastAttempSendOrJoinDate <= date)
                    .OrderBy(x => x.Errors)
                    .Take(maxJoinCount)
                    .ToList();

                // создаем задачи для отправки
                foreach (var bufferEventData in sendEvents)
                {
                    BufferEventData data = bufferEventData;
                    TaskQueue.Add(() => SendOneEvent(data));
                }

                // создаем задачи для склейки
                if (joinEvents.Any(x => x.ComponentControl.Info == null))
                {
                    throw new Exception("joinEvents.Any(x => x.ComponentControl.Info == null)");
                }
                joinEvents = joinEvents.OrderBy(x => x.ComponentControl.Info.Id).ToList();
                const int batchCount = 20;// будем продлевать 20 событий за раз
                var batchEvents = new List<BufferEventData>(batchCount);
                foreach (var bufferEventData in joinEvents)
                {
                    batchEvents.Add(bufferEventData);
                    if (batchEvents.Count == batchCount)
                    {
                        var tempEvents1 = new List<BufferEventData>();
                        tempEvents1.AddRange(batchEvents);
                        TaskQueue.Add(() => JoinEvents(tempEvents1));
                        batchEvents.Clear();
                    }
                }
                if (batchEvents.Count > 0)
                {
                    var tempEvents = new List<BufferEventData>();
                    tempEvents.AddRange(batchEvents);
                    TaskQueue.Add(() => JoinEvents(tempEvents));
                }
            }
            catch (Exception exception)
            {
                Client.InternalLog.Error("Ошибка обработки очереди событий", exception);
            }
        }

        protected virtual void ProcessQueue(object state)
        {
            if (TaskQueue.AllTasks > 0)
            {
                return;
            }
            BeginFlush(DateTime.Now);
        }

        public void Start()
        {
            if (ProcessEventsTimer == null)
            {
                var period = Client.Config.Events.EventManager.SendPeriod;
                ProcessEventsTimer = new Timer(ProcessQueue, null, TimeSpan.Zero, period);
            }
        }

        public void Stop()
        {
            if (ProcessEventsTimer != null)
            {
                ProcessEventsTimer.Dispose();
                ProcessEventsTimer = null;
            }
        }

        public long GetQueueSize()
        {
            return Queue.SizeBytes;
        }

        public void Flush()
        {
            BeginFlush(DateTime.Now);
            TaskQueue.WaitForAllTasksCompleted();
        }
    }
}
