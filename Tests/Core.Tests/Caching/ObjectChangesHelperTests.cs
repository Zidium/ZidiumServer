using System;
using Xunit;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.Core.Caching;
using Zidium.Storage.Ef;

namespace Zidium.Core.Tests.Caching
{
    public class ObjectChangesHelperTests : BaseTest
    {
        [Fact]
        public void MainTest()
        {
            // все пусто
            var a = new DbEvent();
            var b = new DbEvent();
            Assert.False(ObjectChangesHelper.HasChanges(a, b));

            // GUID
            a.Id = Ulid.NewUlid();
            Assert.True(ObjectChangesHelper.HasChanges(a, b));
            b.Id = Ulid.NewUlid();
            Assert.True(ObjectChangesHelper.HasChanges(a, b));
            b.Id = a.Id;
            Assert.False(ObjectChangesHelper.HasChanges(a, b));

            // DateTime?
            a.LastNotificationDate = DateTime.Now;
            Assert.True(ObjectChangesHelper.HasChanges(a, b));
            b.LastNotificationDate = DateTime.Now.AddMilliseconds(1);
            Assert.True(ObjectChangesHelper.HasChanges(a, b));
            b.LastNotificationDate = a.LastNotificationDate;
            Assert.False(ObjectChangesHelper.HasChanges(a, b));

            // string
            a.Message = "123";
            Assert.True(ObjectChangesHelper.HasChanges(a, b));
            b.Message = "3";
            Assert.True(ObjectChangesHelper.HasChanges(a, b));
            b.Message = "12" + "3";
            Assert.False(ObjectChangesHelper.HasChanges(a, b));

            // int
            a.Count = 123;
            Assert.True(ObjectChangesHelper.HasChanges(a, b));
            b.Count = 3;
            Assert.True(ObjectChangesHelper.HasChanges(a, b));
            b.Count = 123;
            Assert.False(ObjectChangesHelper.HasChanges(a, b));

            // enum
            a.Category = EventCategory.ApplicationError;
            Assert.True(ObjectChangesHelper.HasChanges(a, b));
            b.Category = EventCategory.ComponentEvent;
            Assert.True(ObjectChangesHelper.HasChanges(a, b));
            b.Category = EventCategory.ApplicationError;
            Assert.False(ObjectChangesHelper.HasChanges(a, b));
        }
    }
}
