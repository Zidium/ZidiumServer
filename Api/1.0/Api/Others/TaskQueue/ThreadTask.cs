using System;
using System.Threading;

namespace Zidium.Api
{
    public class ThreadTask
    {
        public readonly object Obj;

        protected readonly ThreadStart ThreadStart;

        public readonly string Name;

        protected readonly ParameterizedThreadStart ParameterizedThreadStart;

        public Exception Exception { get; set; }

        public ThreadTask(ParameterizedThreadStart parameterizedThreadStart, object obj)
        {
            if (parameterizedThreadStart == null)
            {
                throw new ArgumentNullException("parameterizedThreadStart");
            }
            Obj = obj;
            ParameterizedThreadStart = parameterizedThreadStart;
        }

        public ThreadTask(ThreadStart threadStart, string name = null)
        {
            if (threadStart == null)
            {
                throw new ArgumentNullException("threadStart");
            }
            ThreadStart = threadStart;
            Name = name;
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
