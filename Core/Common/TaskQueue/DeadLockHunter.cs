using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Logging;

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

        private static readonly ConcurrentDictionary<Guid, ThreadInfo> _threads = new ConcurrentDictionary<Guid, ThreadInfo>();
        private readonly Timer _timer;
        private readonly Zidium.Api.IComponentControl _control;
        private readonly ILogger _logger;

        public DeadLockHunter(Zidium.Api.IComponentControl control, ILogger logger)
        {
            _control = control;
            _logger = logger;
            _timer = new Timer(SearchDeadLock, _timer, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
        }

        protected void SearchDeadLock(object obj)
        {
            try
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

                if (maxDuration > TimeSpan.FromSeconds(30))
                {
                    var exception = new Exception("Long request");
                    _logger.LogWarning(exception, exception.Message);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
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
