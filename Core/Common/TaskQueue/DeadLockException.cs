using System;

namespace Zidium.Core.Common.TaskQueue
{
    public class DeadLockException : Exception
    {
        public DeadLockException(string message, string stack) : base(message)
        {
            Data.Add("DeadlockStack", stack);
        }
    }
}
