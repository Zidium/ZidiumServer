using System;
using System.Threading;

namespace Zidium.Core.Caching
{
    /// <summary>
    /// Блокировка для доступа к объекту кэша
    /// </summary>
    public class CacheLock
    {
        private readonly object _lock = new object();
        private int _lockCount;

        public TimeSpan TimeOut { get; private set; }
        public object Response { get; private set; }
        public Thread Thread { get; private set; }

        public int Count
        {
            get { return _lockCount; }
        }

        /// <summary>
        /// Это ИД блокировки, чтобы не было путаницы с ИД объекта, назвали индексом.
        /// Индекс блокировки уникален для ВСЕХ кэшей.
        /// </summary>
        public long Index { get; protected set; }

        public CacheLock(object response, TimeSpan timeOut)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }
            Index = GetIndex();
            Response = response;
            TimeOut = timeOut;
        }

        public bool TryEnter()
        {
            return Monitor.TryEnter(_lock, TimeSpan.Zero);
        }

        public bool TryEnter(TimeSpan timeOut)
        {
            if (Monitor.TryEnter(_lock, timeOut))
            {
                Interlocked.Increment(ref _lockCount);
                Thread = Thread.CurrentThread;
                //int count = _lockCount;
                //if (count == 1)
                //{
                //    Thread = Thread.CurrentThread;
                //}
                //else
                //{
                //    // проверим, что повторное вхождение сделал тот же поток
                //    // на практике такого случится НЕ должно, но на всякий пожарный проверим
                //    if (Thread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
                //    {
                //        throw new Exception("Thread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId");
                //    }
                //}
                return true;
            }
            return false;
        }

        public void Enter()
        {
            if (TryEnter(TimeOut)==false)
            {
                throw new CacheLockTimeOutException(this);
            }
        }

        public bool IsEntered()
        {
            return Monitor.IsEntered(_lock); 
        }

        public void Exit()
        {
            if (Monitor.IsEntered(_lock))
            {
                Monitor.Exit(_lock);
                Interlocked.Decrement(ref _lockCount);
                if (_lockCount < 0)
                {
                    throw new Exception("_lockCount < 0");
                }
            }
            else
            {
                throw new Exception("Monitor.IsEntered(_lock)==false");
            }
        }

        public bool IsLocked
        {
            get { return _lockCount > 0; }
        }

        private static long _index = 0;

        private static long GetIndex()
        {
            lock (typeof(CacheLock))
            {
                _index++;
                return _index;
            }
        }
    }
}
