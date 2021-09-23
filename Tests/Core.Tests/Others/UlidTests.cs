using System.Threading;
using Xunit;
using Zidium.Common;

namespace Zidium.Core.Tests.Others
{
    public class UlidTests
    {
        [Fact]
        public void NewUlidTest()
        {
            var ulid1 = Ulid.NewUlid();
            Thread.Sleep(1);
            var ulid2 = Ulid.NewUlid();

            var ulid1String = ulid1.ToString();
            var ulid2String = ulid2.ToString();

            Assert.True(string.CompareOrdinal(ulid2String, ulid1String) > 0);
        }
    }
}
