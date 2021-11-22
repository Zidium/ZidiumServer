using System;
using System.Threading;

namespace Zidium.Core.Common
{
    public class SingleThreadWorker
    {
        private readonly Action _action;
        private readonly AutoResetEvent _event = new AutoResetEvent(false);
        private readonly Thread _thread;
        private bool _needStop = false;

        public SingleThreadWorker(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            _action = action;
            _thread = new Thread(DoWrapper);
            _thread.Start();
        }

        protected void DoWrapper()
        {
            while (!_needStop)
            {
                var waitResult = _event.WaitOne(1000);
                try
                {
                    if (waitResult)
                        _action();
                }
                catch (ThreadAbortException)
                {
                    break;
                }
                catch (Exception)
                {
                    // todo наверное имеет смысл делать повторную попытку обработать данные через некоторый интервал
                }
            }
        }

        public void TryStart()
        {
            _event.Set();
        }

        public void Stop()
        {
            _needStop = true;
        }
    }
}
