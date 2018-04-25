using System.Threading;
using Xunit;
using Zidium.Core.Common.Helpers;

namespace Zidium.Core.Tests.Others
{
    public class GetStackTraceTests
    {
        private void DoItem()
        {
            for (int i = 0; i < 1000; i++)
            {

            }
        }

        private void ThreadDo()
        {
            lock (typeof (GetStackTraceTests))
            {
                while (true)
                {
                    DoItem();
                }
            }
        }

        [Fact]
        public void MainTest()
        {
            var thread1 = new Thread(ThreadDo);
            var thread2 = new Thread(ThreadDo);

            // запускаем первый поток
            thread1.Start();
            Thread.Sleep(1000);

            // второй поток должен оказаться в блокировке
            thread2.Start();
            Thread.Sleep(1000);

            Assert.Equal(thread1.ThreadState, ThreadState.Running);
            Assert.Equal(thread2.ThreadState, ThreadState.WaitSleepJoin);

            for (int i = 0; i < 20; i++)
            {
                var stack1 = ThreadHelper.GetStackTraceInternal(thread1);
                Assert.NotNull(stack1);

                var stack2 = ThreadHelper.GetStackTraceInternal(thread2);
                Assert.NotNull(stack2);

                Thread.Sleep(300);
            }

            thread1.Abort();
            thread2.Abort();
        }
    }
}
