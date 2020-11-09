using System.Threading;
using Xunit;
using Zidium.Core.Common;

namespace Zidium.Core.Tests.Caching
{
    public class SingleThreadWorkerTest : BaseTest
    {
        protected void Do()
        {
            lock (this)
            {
                Thread.Sleep(10);
            }    
        }

        [Fact]
        public void MainTest()
        {
            int threadsCount = 50;
            int processCount = 1000*1000;
            var woker = new SingleThreadWorker(Do);

            // запускаем потоки
            var threads = new Thread[threadsCount];
            for (int i = 0; i < threadsCount; i++)
            {
                var thread = new Thread(() =>
                {
                    for (int j = 0; j < processCount; j++)
                    {
                        woker.TryStart();
                    }
                });
                thread.Start();
                threads[i] = thread;
            }

            // ждем выполнения всех потоков
            foreach (var thread in threads)
            {
                thread.Join();
            }
        }
    }
}
