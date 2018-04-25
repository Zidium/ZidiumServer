using System;
using System.Threading;

namespace Zidium.Core.Common
{
    public class SingleThreadWorker
    {
        private object _lock = new object();
        private Action _action;
        private Thread _thread;
        private DateTime _lastRequestTime;

        public SingleThreadWorker(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            _action = action;
        }

        public bool IsWorking
        {
            get { return _thread != null; }
        }

        public Thread Thread
        {
            get { return _thread; }
        }

        protected void DoWrapper()
        {
            try
            {
                var startTime = DateTime.Now;
                _thread = Thread.CurrentThread;
                while (true)
                {
                    try
                    {
                        _action();
                    }
                    catch (ThreadAbortException) { break; }
                    catch(Exception)
                    {
                        // todo наверное имеет смысл делать повторную попытку обработать данные через некоторый интервал
                    } 
                    lock (_lock)
                    {
                        if (_lastRequestTime < startTime)
                        {
                            _thread = null;
                            return;
                        }
                        startTime = DateTime.Now;
                    }
                }
            }
            finally
            {
                _thread = null;
            }
        }

        public void TryStart()
        {
            lock (_lock)
            {
                _lastRequestTime = DateTime.Now;
                if (_thread == null)
                {
                    var thread = new Thread(DoWrapper);
                    thread.Start();

                    // придержим блокировку до запуска потока
                    for (int i = 0; i < 100; i++)
                    {
                        if (_thread != null)
                        {
                            return;
                        }
                        Thread.Sleep(10);
                    }
                }
            }
        }
    }
}
