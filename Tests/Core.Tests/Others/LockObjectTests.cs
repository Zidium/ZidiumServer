using System;
using Xunit;
using Zidium.Core.Common;

namespace Zidium.Core.Tests.Others
{
    public class LockObjectTests
    {
        [Fact]
        public void GetByGuid()
        {
            var lockObj = new LockObject(1000);
            var guid1 = new Guid("5A79EF6A-F8DF-496B-BB64-5C3B7F8E61A3");
            var guid2 = Guid.Parse("5A79EF6A-F8DF-496B-BB64-5C3B7F8E61A3");
            var guid3 = guid2; // копия структуры
            var guid4 = new Guid("51094EE8-30F5-49EC-9A5A-94ABA9E56A8A");
            
            var obj1 = lockObj.Get(guid1);
            var obj2 = lockObj.Get(guid2);
            var obj3 = lockObj.Get(guid3);
            var obj4 = lockObj.Get(guid4);

            Assert.True(ReferenceEquals(obj1, obj2));
            Assert.True(ReferenceEquals(obj1, obj3));
            Assert.False(ReferenceEquals(obj1, obj4));

            int count = 100;
            for (int i = 0; i < count; i++)
            {
                var g1 = Guid.NewGuid();
                var g2 = Guid.Parse(g1.ToString());
                var g3 = g1;

                var o1 = lockObj.Get(g1);
                var o2 = lockObj.Get(g2);
                var o3 = lockObj.Get(g3);

                Assert.True(ReferenceEquals(o1, o2));
                Assert.True(ReferenceEquals(o1, o3));
            }
        }
    }
}
