using System;
using Xunit;
using Zidium.Storage.Ef;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Others
{
    public class DbDateTimeTests : BaseTest
    {
        [Fact]
        public void SaveLocalDateTimeTest()
        {
            var account = TestHelper.GetTestAccount();

            Guid id;
            using (var context = account.GetDbContext())
            {
                var metricType = new DbMetricType()
                {
                    Id = Guid.NewGuid(),
                    SystemName = Guid.NewGuid().ToString(),
                    DisplayName = Guid.NewGuid().ToString(),
                    CreateDate = new DateTime(2020, 12, 31, 10, 00, 00, DateTimeKind.Local)
                };

                context.MetricTypes.Add(metricType);
                context.SaveChanges();

                id = metricType.Id;
            }

            using (var context = account.GetDbContext())
            {
                var metricType = context.MetricTypes.Find(id);

                Assert.Equal(new DateTime(2020, 12, 31, 10, 00, 00, DateTimeKind.Local), metricType.CreateDate);
                Assert.Equal(DateTimeKind.Unspecified, metricType.CreateDate.Value.Kind);
            }
        }

        [Fact]
        public void SaveUnspecifiedDateTimeTest()
        {
            var account = TestHelper.GetTestAccount();

            Guid id;
            using (var context = account.GetDbContext())
            {
                var metricType = new DbMetricType()
                {
                    Id = Guid.NewGuid(),
                    SystemName = Guid.NewGuid().ToString(),
                    DisplayName = Guid.NewGuid().ToString(),
                    CreateDate = new DateTime(2020, 12, 31, 10, 00, 00)
                };

                context.MetricTypes.Add(metricType);
                context.SaveChanges();

                id = metricType.Id;
            }

            using (var context = account.GetDbContext())
            {
                var metricType = context.MetricTypes.Find(id);

                Assert.Equal(new DateTime(2020, 12, 31, 10, 00, 00), metricType.CreateDate);
                Assert.Equal(DateTimeKind.Unspecified, metricType.CreateDate.Value.Kind);
            }
        }

        [Fact]
        public void SaveUtcDateTimeTest()
        {
            var account = TestHelper.GetTestAccount();

            Guid id;
            using (var context = account.GetDbContext())
            {
                var metricType = new DbMetricType()
                {
                    Id = Guid.NewGuid(),
                    SystemName = Guid.NewGuid().ToString(),
                    DisplayName = Guid.NewGuid().ToString(),
                    CreateDate = new DateTime(2020, 12, 31, 10, 00, 00, DateTimeKind.Utc)
                };

                context.MetricTypes.Add(metricType);
                context.SaveChanges();

                id = metricType.Id;
            }

            using (var context = account.GetDbContext())
            {
                var metricType = context.MetricTypes.Find(id);

                Assert.Equal(new DateTime(2020, 12, 31, 10, 00, 00, DateTimeKind.Utc), metricType.CreateDate);
                Assert.Equal(DateTimeKind.Unspecified, metricType.CreateDate.Value.Kind);
            }
        }
    }
}
