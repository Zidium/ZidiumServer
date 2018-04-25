using System;
using System.Threading;

namespace Zidium.Core.Common
{
    public class ThreadTask
    {
        public readonly object Obj;

        protected readonly ThreadStart ThreadStart;

        protected readonly ParameterizedThreadStart ParameterizedThreadStart;

        public Exception Exception { get; set; }

        public ThreadTask(object obj, ParameterizedThreadStart parameterizedThreadStart)
        {
            if (parameterizedThreadStart == null)
            {
                throw new ArgumentNullException("parameterizedThreadStart");
            }
            Obj = obj;
            ParameterizedThreadStart = parameterizedThreadStart;
        }

        public ThreadTask(ThreadStart threadStart)
        {
            if (threadStart == null)
            {
                throw new ArgumentNullException("threadStart");
            }
            ThreadStart = threadStart;
        }

        public virtual void Execute()
        {
            if (ThreadStart != null)
            {
                ThreadStart.Invoke();
            }
            else
            {
                ParameterizedThreadStart.Invoke(Obj);
            }
        }
    }
}
