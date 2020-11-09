using System.Threading;
using Zidium.Core.Common;
using Xunit;

namespace Zidium.Core.Tests.Others
{
    public class IncrementTests : BaseTest
    {
        public int Value = 0;

        [Fact]
        public void InterlockedTest()
        {
            int threads = 4;
            var tasks = new ThreadTaskQueue(threads);
            tasks.UseSystemThreadPool = false;
            for (int thread = 0; thread < threads; thread++)
            {
                tasks.Add(() =>
                {
                    int count = 100 * 1000 * 1000;
                    for (int i = 0; i < count; i++)
                    {
                        Interlocked.Increment(ref Value);
                    }
                    for (int i = 0; i < count; i++)
                    {
                        Interlocked.Decrement(ref Value);
                    }
                });
            }
            
            tasks.WaitForAllTasksCompleted();
            Assert.Equal(0, Value);
        }
    }
}
