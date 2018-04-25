using System.Linq;
using NUnit.Framework;

namespace AppMonitoring.Tests.Others
{
    [TestFixture]
    public class VersionTests
    {
        [Ignore]
        [Test]
        public void Test()
        {
            var unsorted = new[]
            {
                "10.1.1", 
                "10.0.100",
                "9.900.900",
                "1.222.0",
                "9.3",
                "0.0.0",
                "3"
            };
            var sorted = new[]
            {
                "0.0.0",
                "1.222.0",
                "3",
                "9.3",
                "9.900.900",
                "10.0.100",
                "10.1.1"
            };
            var sorted2 = unsorted.OrderBy(x => x).ToArray();
            Assert.AreEqual(sorted.Length, unsorted.Length);
            for (int i = 0; i < sorted.Length; i++)
            {
                var a = sorted[i];
                var b = sorted2[i];
                Assert.AreEqual(a, b);
            }
        }
    }
}
