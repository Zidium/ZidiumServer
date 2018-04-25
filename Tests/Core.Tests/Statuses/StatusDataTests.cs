using System;
using System.Linq;
using Xunit;
using Zidium.Api;
using Zidium.Core.AccountsDb;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Statuses
{
    public class StatusDataTests
    {
        private void CheckReasonAndStatusEvents(Event reason, Event status)
        {
            Assert.Equal(reason.Id, status.FirstReasonEventId);
            Assert.True(reason.StatusEvents.Any(x => x.Id == status.Id));
            Assert.True(status.ReasonEvents.Any(x => x.Id == reason.Id));
        }

        [Fact]
        public void Statuses()
        {
            var account = TestHelper.GetTestAccount();
            var componentControl = account.CreateRandomComponentControl();
            Assert.False(componentControl.IsFake());
            account.SaveAllCaches();

            var error = componentControl.CreateApplicationError("Error", "error msg")
                    .SetImportance(EventImportance.Success)
                    .SetJoinInterval(TimeSpan.FromHours(1));

            var response1 = error.Send();
            response1.Check();

            var response2 = error.Send();
            response2.Check();
            Assert.Equal(response1.Data.EventId, response2.Data.EventId);

            Guid eventId = response2.Data.EventId;
            account.SaveAllCaches();

            using (var accountDbContext = account.CreateAccountDbContext())
            {
                var component = accountDbContext.Components.Single(x => x.Id == componentControl.Info.Id);
                // ошибка
                var eventObj = accountDbContext.Events.Single(x => x.Id == eventId);
                Assert.Equal(2, eventObj.Count);
                Assert.NotNull(eventObj.LastStatusEventId);
                Assert.Equal(1, eventObj.StatusEvents.Count);

                // колбаса ошибок
                var eventsStatus = component.EventsStatus;
                Assert.Equal(eventsStatus.FirstEventId, eventId);
                Assert.Equal(eventsStatus.LastEventId, eventId);
                Assert.Equal(eventsStatus.StatusEventId, eventObj.LastStatusEventId);
                var eventsStatusEvent = accountDbContext.Events.Single(x => x.Id == eventsStatus.StatusEventId);
                Assert.Equal(1, eventsStatusEvent.StatusEvents.Count);
                Assert.Equal(eventsStatusEvent.StatusEvents.First().Category, Core.Api.EventCategory.ComponentInternalStatus);
                Assert.Equal(eventsStatusEvent.ReasonEvents.First().Category, Core.Api.EventCategory.ApplicationError);
                Assert.Equal(eventsStatus.FirstEventId, eventsStatusEvent.ReasonEvents.First().Id);
                Assert.True(eventsStatusEvent.FirstReasonEventId.HasValue);
                Assert.True(eventsStatus.FirstEventId.HasValue);
                Assert.Equal(eventsStatusEvent.FirstReasonEventId.Value, eventsStatus.FirstEventId.Value);
                CheckReasonAndStatusEvents(eventObj, eventsStatusEvent);

                // колбаса внутреннего статуса
                var internalStatus = component.InternalStatus;
                Assert.Equal(internalStatus.FirstEventId, eventsStatusEvent.Id);
                Assert.Equal(internalStatus.LastEventId, eventsStatusEvent.Id);
                Assert.Equal(internalStatus.StatusEventId, eventsStatusEvent.LastStatusEventId);
                var internalStatusEvent = accountDbContext.Events.Single(x => x.Id == internalStatus.StatusEventId);
                Assert.Equal(1, internalStatusEvent.StatusEvents.Count);
                CheckReasonAndStatusEvents(eventsStatusEvent, internalStatusEvent);

                // колбаса внешнего статуса
                var externalStatus = component.ExternalStatus;
                Assert.Equal(externalStatus.FirstEventId, internalStatusEvent.Id);
                Assert.Equal(externalStatus.LastEventId, internalStatusEvent.Id);
                Assert.Equal(externalStatus.StatusEventId, internalStatusEvent.LastStatusEventId);
                var externalStatusEvent = accountDbContext.Events.Single(x => x.Id == externalStatus.StatusEventId);
                Assert.Equal(1, externalStatusEvent.ReasonEvents.Count);
                CheckReasonAndStatusEvents(internalStatusEvent, externalStatusEvent);

                // колбаса дочерних компонентов
                var childsStatus = component.Parent.ChildComponentsStatus;
                Assert.Equal(externalStatusEvent.Id, childsStatus.FirstEventId);
                Assert.Equal(externalStatusEvent.Id, childsStatus.LastEventId);
                Assert.Equal(externalStatusEvent.LastStatusEventId, childsStatus.StatusEventId);
                var childsStatusEvent = accountDbContext.Events.Single(x => x.Id == childsStatus.StatusEventId);
                Assert.Equal(1, childsStatusEvent.StatusEvents.Count);
                CheckReasonAndStatusEvents(externalStatusEvent, childsStatusEvent);

            }
        }
    }
}
