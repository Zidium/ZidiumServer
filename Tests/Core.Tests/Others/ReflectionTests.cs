using System.Reflection;
using Xunit;
using Zidium.Core.Api;

namespace Zidium.Core.Tests.Others
{
    public class ReflectionTests
    {
        [Fact]
        public void GetMethodSaveAllCachesTest()
        {
            var action = "saveAllCaches";
            var method = typeof(DispatcherWrapper).GetMethod(action, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase);
            Assert.NotNull(method);
        }

        [Fact]
        public void GetMethodGetEchoTest()
        {
            var action = "getEcho";
            var method = typeof(DispatcherWrapper).GetMethod(action, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase);
            Assert.NotNull(method);
        }
    }
}
