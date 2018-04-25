using System;
using System.Data.Entity;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Caching
{
    public class EfTests
    {
        [Fact]
        public void AttachTest()
        {
            var account = TestHelper.GetTestAccount();
            var eventType = TestHelper.GetTestEventType(account.Id);

            Event event1;
            using (var accountDbContext = account.CreateAccountDbContext())
            {
                var now = DateTime.Now;
                event1 = new Event()
                {
                    ActualDate = now,
                    CreateDate = now,
                    EndDate = now,
                    Id = Guid.NewGuid(),
                    LastUpdateDate = now,
                    StartDate = now ,
                    EventTypeId = eventType.Id
                };
                accountDbContext.Events.Add(event1);
                accountDbContext.SaveChanges();
            }

            using (var accountDbContext = account.CreateAccountDbContext())
            {
                accountDbContext.Events.Attach(event1);
                var entity = accountDbContext.Entry(event1);
                accountDbContext.ChangeTracker.DetectChanges();
                Assert.Equal(EntityState.Unchanged, entity.State);
                event1.LastUpdateDate = event1.LastUpdateDate.AddSeconds(1);
                entity = accountDbContext.Entry(event1);

                // проверим что LastUpdateDate изменился
                accountDbContext.ChangeTracker.DetectChanges();
                Assert.Equal(EntityState.Modified, entity.State);
                var old = entity.OriginalValues.GetValue<DateTime>("LastUpdateDate");
                var current = entity.CurrentValues.GetValue<DateTime>("LastUpdateDate");
                Assert.NotEqual(old, current);

                // проверим что StartDate НЕ изменился
                old = entity.OriginalValues.GetValue<DateTime>("StartDate");
                current = entity.CurrentValues.GetValue<DateTime>("StartDate");
                Assert.Equal(old, current);
                accountDbContext.SaveChanges();
            }
            
        }
    }
}
