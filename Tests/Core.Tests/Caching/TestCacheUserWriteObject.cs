using Zidium.Core.Caching;

namespace Zidium.Core.Tests.Caching
{
    public class TestCacheUserWriteObject : CacheWriteObjectBase<CacheRequest, TestCacheUserResponse, ITestCacheUserReadObject, TestCacheUserWriteObject>, ITestCacheUserReadObject
    {
        public override int GetCacheSize()
        {
            return 1;
        }

        public string Name { get; set; }

        public int SaveCount { get; set; }

        public int LoadCount { get; set; }

        public int Money { get; set; }

        public int Money2 { get; set; }

        public bool IsHot { get; set; }
    }
}
