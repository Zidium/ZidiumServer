using System;
using Xunit;
using Zidium.TestTools;

namespace Zidium.Api.Tests.EventManagerTests
{
    public class ClearEventsQueueTests : BaseTest
    {
        /// <summary>
        /// Проверяет, что очередь событий не будет бесконечно расти, а имеет ограничение
        /// </summary>
        [Fact]
        public void TestOffline()
        {
            var client = TestHelper.GetOfflineClient();
            var component = client.GetRootComponentControl();

            int count = 1000;
            client.Config.Events.EventManager.QueueBytes = 5000;
            for (int i = 0; i < count; i++)
            {
                var eventData = component.CreateComponentEvent("test " + Guid.NewGuid());
                eventData.Properties["key1"] = new String('x', 500);
                eventData.Add();
            }
            var queue = (client.EventManager as EventManager).Queue;

            Assert.True(queue.SizeBytes > 2500);
            Assert.True(queue.SizeBytes < 5000);
            Assert.True(queue.Count() < count);
        }
    }
}
