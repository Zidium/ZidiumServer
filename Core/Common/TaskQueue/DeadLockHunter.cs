using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Zidium.Core.Common.Helpers;

namespace Zidium.Core.Common.TaskQueue
{
    /// <summary>
    /// Класс отслеживает все потоки, работающие дольше заданного времени
    /// </summary>
    public class DeadLockHunter
    {
        private class ThreadInfo
        {
            public Guid Id { get; set; }

            public Thread Thread { get; set; }

            public string Name { get; set; }

            public DateTime StartDate { get; set; }
        }

        private static ConcurrentDictionary<Guid, ThreadInfo> _threads = new ConcurrentDictionary<Guid, ThreadInfo>();

        private Timer _timer;

        private Zidium.Api.IComponentControl _control;

        public DeadLockHunter(Zidium.Api.IComponentControl control)
        {
            _control = control;
            _timer = new Timer(SearchDeadLock, _timer, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
        }

        protected void SearchDeadLock(object obj)
        {
            TimeSpan maxDuration = TimeSpan.Zero;
            foreach (var threadInfo in _threads.Values)
            {
                var duration = DateTime.Now - threadInfo.StartDate;

                /*
                if (duration > TimeSpan.FromSeconds(10))
                {
                    var stack = ThreadHelper.GetStackText(threadInfo.Thread);
                    var exception = new DeadLockException("DEADLOCK " + threadInfo.Name, stack);
                    _control.Log.Fatal(exception);
                }
                */

                if (duration > maxDuration)
                    maxDuration = duration;
            }

            //_control.Log.Debug("DeadLockHunter Max Thread Duration: " + maxDuration + ", ThreadCount: " + _threads.Count);

            // Если хоть один поток завис, то выводим стеки всех зависших потоков, для анализа
            if (maxDuration > TimeSpan.FromSeconds(30))
            {
                var exception = new Exception("Long request");
                var properties = new Dictionary<string, object>();
                foreach (var threadInfo in _threads.Values.OrderBy(t => t.StartDate))
                {
                    var duration = DateTime.Now - threadInfo.StartDate;
                    if (duration > TimeSpan.FromSeconds(30))
                    {
                        var stack = ThreadHelper.GetStackText(threadInfo.Thread);
                        properties.Add("Stack " + threadInfo.Thread.ManagedThreadId, threadInfo.Name + ": " + stack);
                    }
                }
                _control.Log.Warning(exception.Message, exception, properties);
            }
        }

        public Guid Add(string threadName)
        {
            var thread = Thread.CurrentThread;
            var id = Guid.NewGuid();
            var threadInfo = new ThreadInfo()
            {
                Id = id,
                StartDate = DateTime.Now,
                Thread = thread,
                Name = threadName
            };
            _threads.TryAdd(id, threadInfo);
            return id;
        }

        public void Remove(Guid id)
        {
            _threads.TryRemove(id, out _);
        }
    }
}
