using System.Collections.Generic;
using System.Linq;

namespace Zidium.Api
{
    public class WebLogQueue:IWebLogQueue
    {
        protected List<WebLogMessage> Messages { get; set; }

        protected int AllBytes;

        public object SynchRoot
        {
            get { return this; }
        }

        public WebLogQueue()
        {
            Messages = new List<WebLogMessage>();
        }

        public void Add(WebLogMessage message)
        {
            if (message != null)
            {
                lock (this)
                {
                    Messages.Add(message);
                    AllBytes += message.Size;
                }
            }
        }

        protected virtual List<WebLogMessage> GetSortedMessages()
        {
            return Messages
                    .OrderByDescending(x => x.Level)
                    .ThenBy(x => x.Attemps)
                    .ThenBy(x => x.LastAttempTime)
                    .ToList();
        } 

        public List<WebLogMessage> GetBatchMessages(int batchBytes)
        {
            lock (this)
            {
                var sortedMessages = GetSortedMessages();

                int size = 0;
                var batch = new List<WebLogMessage>();
                foreach (var message in sortedMessages)
                {
                    if (message.ComponentControl.IsFake())
                    {
                        continue;
                    }
                    size += message.Size;
                    if (size >= batchBytes)
                    {
                        if (batch.Count == 0)
                        {
                            batch.Add(message);
                        }
                        break;
                    }
                    batch.Add(message);
                }
                foreach (var message in batch)
                {
                    Messages.Remove(message);
                    AllBytes -= message.Size;
                }
                return batch;
            }
        }

        public void AddRange(List<WebLogMessage> messages)
        {
            lock (this)
            {
                int size = messages.Sum(x => x.Size);
                Messages.AddRange(messages);
                AllBytes += size;
            }
        }


        public int Count()
        {
            return Messages.Count;
        }

        public int Bytes()
        {
            return AllBytes;
        }

        public void ClearBySize(int queueBytes)
        {
            lock (this)
            {
                int size = 0;
                var messages = GetSortedMessages();
                var batch = new List<WebLogMessage>();
                foreach (var message in messages)
                {
                    size += message.Size;
                    if (size > queueBytes)
                    {
                        break;
                    }
                    batch.Add(message);
                }
                Messages = batch;
            }
        }
    }
}
