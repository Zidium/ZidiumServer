using System;
using System.Threading.Tasks;
using Zidium.Core.Common;
using Xunit;

namespace Zidium.Core.Tests.Others
{
    public class HashHelperTests : BaseTest
    {
        [Fact]
        public void MultiThreadedHashTest()
        {
            var rnd = new Random();
            var s = new string[100000];
            for (var i = 0; i < s.Length; i++)
                s[i] = rnd.Next(int.MaxValue).ToString();
            Parallel.ForEach(s, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, t => HashHelper.GetMD5(t));
        }
    }
}
