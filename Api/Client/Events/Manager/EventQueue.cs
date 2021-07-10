using System;
using System.Collections.Generic;
using System.Linq;

namespace Zidium.Api
{
    public class EventQueue:IEventQueue
    {
        protected Dictionary<long, List<BufferEventData>> Events { get; set; }

        public int SizeBytes { get; protected set; }

        public int MaxSizeBytes { get; set; }

        protected Client Client { get; set; }

        public EventQueue(Client client)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }
            Client = client;
            Events = new Dictionary<long, List<BufferEventData>>();
            MaxSizeBytes = 1024*1024*100; // 100 Mbytes
        }

        public void Add(BufferEventData bufferEvent)
        {
            lock (SynchRoot)
            {
                List<BufferEventData> list = null;
                if (Events.TryGetValue(bufferEvent.GlobalJoinKey, out list) == false)
                {
                    list = new List<BufferEventData>();
                    Events.Add(bufferEvent.GlobalJoinKey, list);
                }
                list.Add(bufferEvent);
                bufferEvent.Size = 0;
                UpdateSize(bufferEvent);
            }
        }
        

        public int Count()
        {
            return Events.Count;
        }

        
        public object SynchRoot
        {
            get { return this; }
        }

        public List<BufferEventData> GetAllByGlobalJoinKey(long globalJoinKey)
        {
            lock (SynchRoot)
            {
                List<BufferEventData> list = null;
                if (Events.TryGetValue(globalJoinKey, out list))
                {
                    return list;
                }
                return new List<BufferEventData>(0);
            }
        }

        public List<BufferEventData> GetAll()
        {
            lock (SynchRoot)
            {
                var events = Events
                    .SelectMany(pair => pair.Value)
                    .ToList();

                return events;
            }
        }

        public void UpdateSize(BufferEventData bufferEvent)
        {
            lock (SynchRoot)
            {
                int oldSize = bufferEvent.Size;
                bufferEvent.Size = bufferEvent.GetSize();
                SizeBytes -= oldSize;
                SizeBytes += bufferEvent.Size;
                if (SizeBytes < 0)
                {
                    SizeBytes = 0;
                }
                if (SizeBytes > MaxSizeBytes)
                {
                    Client.InternalLog.Warning("Очищаем очередь событий");
                    var allEvents = GetAll();
                    allEvents = allEvents.OrderBy(x => x.LastAttempSendOrJoinDate).ToList();
                    foreach (var bufferEventData in allEvents)
                    {
                        // удаляем события пока размер очереди не будет меньше половины максимума
                        if (SizeBytes < MaxSizeBytes/2)
                        {
                            break;
                        }
                        List<BufferEventData> list = null;
                        if (Events.TryGetValue(bufferEventData.GlobalJoinKey, out list))
                        {
                            if (list.Remove(bufferEventData))
                            {
                                SizeBytes -= bufferEvent.Size;
                                if (list.Count == 0)
                                {
                                    Events.Remove(bufferEventData.GlobalJoinKey);
                                }
                            }
                        }
                    }
                    if (Count() == 0)
                    {
                        SizeBytes = 0;
                    }
                    Client.InternalLog.Debug("Очередь событий очищена");
                }
            }
        }
    }
}
