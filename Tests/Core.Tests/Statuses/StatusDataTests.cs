using System;
using System.Linq;
using Xunit;
using Zidium.Api.Dto;
using Zidium.Storage.Ef;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Statuses
{
    public class StatusDataTests : BaseTest
    {
        [Fact]
        public void Statuses()
        {
            var account = TestHelper.GetTestAccount();
            var parentComponentControl = account.CreateRandomComponentControl();
            Assert.False(parentComponentControl.IsFake());
            var componentControl = account.CreateRandomComponentControl(parentComponentControl);
            account.SaveAllCaches();

            var error = componentControl.CreateApplicationError("Error", "error msg")
                    .SetImportance(EventImportance.Success)
                    .SetJoinInterval(TimeSpan.FromHours(1));

            var response1 = error.Send();
            response1.Check();

            var response2 = error.Send();
            response2.Check();
            Assert.Equal(response1.GetDataAndCheck().EventId, response2.GetDataAndCheck().EventId);

            Guid eventId = response2.GetDataAndCheck().EventId;
            account.SaveAllCaches();

            using (var accountDbContext = account.GetDbContext())
            {
                var component = accountDbContext.Components.Single(x => x.Id == componentControl.Info.Id);
                // ошибка
                var eventObj = accountDbContext.Events.Single(x => x.Id == eventId);
                Assert.Equal(2, eventObj.Count);
                Assert.NotNull(eventObj.LastStatusEventId);
                Assert.Equal(1, eventObj.StatusEvents.Count);

                // колбаса ошибок
                var eventsBulb = component.EventsStatus;
                Assert.Equal(eventsBulb.FirstEventId, eventId);
                Assert.Equal(eventsBulb.LastEventId, eventId);
                Assert.Equal(eventsBulb.StatusEventId, eventObj.LastStatusEventId);
                var eventsBulbStatus = accountDbContext.Events.Single(x => x.Id == eventsBulb.StatusEventId);
                Assert.Equal(1, eventsBulbStatus.StatusEvents.Count);
                Assert.Equal(EventCategory.ComponentInternalStatus, eventsBulbStatus.StatusEvents.First().Category);
                Assert.Equal(EventCategory.ApplicationError, eventsBulbStatus.ReasonEvents.First().Category);
                Assert.Equal(eventsBulb.FirstEventId, eventsBulbStatus.ReasonEvents.First().Id);
                Assert.True(eventsBulbStatus.FirstReasonEventId.HasValue);
                Assert.True(eventsBulb.FirstEventId.HasValue);
                Assert.Equal(eventsBulbStatus.FirstReasonEventId.Value, eventsBulb.FirstEventId.Value);
                CheckReasonAndStatusEvents(eventObj, eventsBulbStatus);

                // колбаса внутреннего статуса
                var internalStatus = component.InternalStatus;
                Assert.Equal(internalStatus.FirstEventId, eventsBulbStatus.Id);
                Assert.Equal(internalStatus.LastEventId, eventsBulbStatus.Id);
                Assert.Equal(internalStatus.StatusEventId, eventsBulbStatus.LastStatusEventId);
                var internalStatusEvent = accountDbContext.Events.Single(x => x.Id == internalStatus.StatusEventId);
                Assert.Equal(1, internalStatusEvent.StatusEvents.Count);
                CheckReasonAndStatusEvents(eventsBulbStatus, internalStatusEvent);

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

        private void CheckReasonAndStatusEvents(DbEvent reason, DbEvent status)
        {
            Assert.Equal(reason.Id, status.FirstReasonEventId);
            Assert.True(reason.StatusEvents.Any(x => x.Id == status.Id));
            Assert.True(status.ReasonEvents.Any(x => x.Id == reason.Id));
        }

    }
}
