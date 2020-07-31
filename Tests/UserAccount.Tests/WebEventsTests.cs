using System;
using System.Web.Mvc;
using Zidium.Core.Api;
using Xunit;
using Zidium.Storage;
using Zidium.TestTools;
using Zidium.UserAccount.Controllers;
using Zidium.UserAccount.Models.Events;

namespace Zidium.UserAccount.Tests
{
    public class WebEventsTests
    {
        [Fact]
        public void ChangeImportanceTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var eventType = TestHelper.GetTestEventType(account.Id);
            var component = account.CreateRandomComponentControl();
            var dispatcher = DispatcherHelper.GetDispatcherService();
            var eventRequest = new SendEventRequest()
            {
                Token = account.GetCoreToken(),
                Data = new SendEventData()
                {
                    ComponentId = component.Info.Id,
                    TypeSystemName = eventType.SystemName,
                    Category = EventCategory.ComponentEvent,
                    Version = "1.2.3.4",
                    JoinInterval = TimeSpan.FromSeconds(0).TotalSeconds
                }
            };
            var sendEventResponse = dispatcher.SendEvent(eventRequest);
            Assert.True(sendEventResponse.Success);
            var eventId = sendEventResponse.Data.EventId;
            var eventResponse = dispatcher.GetEventById(new GetEventByIdRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetEventByIdRequestData()
                {
                    EventId = eventId
                }
            });
            Assert.True(eventResponse.Success);
            var event_ = eventResponse.Data;

            ChangeImportanceModel model;
            using (var controller = new EventsController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.ChangeImportance(eventId);
                model = (ChangeImportanceModel)result.Model;
            }
            model.Importance = EventImportance.Warning;
            using (var controller = new EventsController(account.Id, user.Id))
            {
                controller.ChangeImportance(model);
            }
            using (var accountContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var dbEventType = accountContext.EventTypes.Find(model.EventTypeId);
                Assert.NotNull(dbEventType);
                Assert.Equal(model.EventTypeId, dbEventType.Id);
                Assert.Equal(model.Version, event_.Version);
                Assert.Equal(model.Importance, dbEventType.ImportanceForOld);
            }
        }

        [Fact]
        public void DeletedComponentTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);

            // Создадим компонент
            var component = account.CreateRandomComponentControl();

            // Создадим тип события
            var eventType = TestHelper.GetTestEventType(account.Id);

            // Создадим событие, актуальное 1 день
            var dispatcher = DispatcherHelper.GetDispatcherService();
            var eventRequest = new SendEventRequest()
            {
                Token = account.GetCoreToken(),
                Data = new SendEventData()
                {
                    ComponentId = component.Info.Id,
                    TypeSystemName = eventType.SystemName,
                    Category = EventCategory.ComponentEvent,
                    Version = "1.2.3.4",
                    JoinInterval = TimeSpan.FromSeconds(0).TotalSeconds,
                }
            };
            var sendEventResponse = dispatcher.SendEvent(eventRequest);
            Assert.True(sendEventResponse.Success);

            // Удалим компонент
            var deleteComponentResponse = dispatcher.DeleteComponent(new DeleteComponentRequest()
            {
                Token = account.GetCoreToken(),
                Data = new DeleteComponentRequestData()
                {
                    ComponentId = component.Info.Id
                }
            });
            Assert.True(deleteComponentResponse.Success);

            // Проверим, что список событий открывается без ошибки
            using (var controller = new EventsController(account.Id, user.Id))
            {
                var viewResult = (ViewResultBase) controller.Index();
            }
        }


    }
}
