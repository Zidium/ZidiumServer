using System;
using System.Diagnostics;
using System.Threading;

namespace Zidium.Core.Common.Helpers
{
    public static class ThreadHelper
    {
        public static string GetStackText(Thread thread)
        {
            if (thread == null)
            {
                return "thread is null";
            }
            var stackTrace = GetStackTraceInternal(thread);
            if (stackTrace == null)
            {
                return "stackTrace is null";
            }
            return stackTrace.ToString();
        }

        public static StackTrace GetStackTraceInternal(Thread targetThread)
        {
            StackTrace stackTrace = null;
            var ready = new ManualResetEventSlim();

            new Thread(() =>
            {
                // Backstop to release thread in case of deadlock:
                ready.Set();
                Thread.Sleep(5000);
                try
                {
#pragma warning disable 618
                    targetThread.Resume();
#pragma warning restore 618
                }
                catch
                {
                }
            }).Start();

            ready.Wait();

            // обернем в try-catch, чтобы не получить ошибку "System.Threading.ThreadStateException: Поток не выполняется; его нельзя приостановить"
            try
            {
#pragma warning disable 618
                targetThread.Suspend();
#pragma warning restore 618
            }
            catch
            {
            }

            try
            {
#pragma warning disable 618
                stackTrace = new StackTrace(targetThread, true);
#pragma warning restore 618
            }
            catch (Exception exception)
            {
                return new StackTrace(exception, true); 
                /* Deadlock */
            }
            finally
            {
                try
                {
#pragma warning disable 618
                    targetThread.Resume();
#pragma warning restore 618
                }
                catch
                {
                    //stackTrace = null;  /* Deadlock */
                }
            }

            return stackTrace;
        }
    }
}
