using System;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Xunit;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;
using Zidium.TestTools;
using Zidium.UserAccount.Controllers;
using Zidium.UserAccount.Helpers;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Tests
{
    public class WebReportsTests
    {
        [Fact]
        public void StartsReportTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var component = account.CreateRandomComponentControl();
            var dispatcher = DispatcherHelper.GetDispatcherService();
            var yesterdayDate = DateTime.Now.AddDays(-1);

            // Отправим событие о запуске вчера
            var eventType = TestHelper.GetOrCreateEventType(
                account.Id, 
                EventCategory.ComponentEvent, 
                SystemEventType.ComponentStart);

            var eventRequestYesterday = new SendEventRequest()
            {
                Token = account.GetCoreToken(),
                Data = new SendEventData()
                {
                    ComponentId = component.Info.Id,
                    StartDate = yesterdayDate,
                    TypeSystemName = eventType.SystemName,
                    Category = EventCategory.ComponentEvent,
                    Version = "1.2.3.4",
                    JoinInterval = TimeSpan.FromSeconds(0).TotalSeconds
                }
            };
            var sendEventResponseYesterday = dispatcher.SendEvent(eventRequestYesterday);
            sendEventResponseYesterday.Check();
            var eventResponseYesterday = dispatcher.GetEventById(new GetEventByIdRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetEventByIdRequestData()
                {
                    EventId = sendEventResponseYesterday.Data.EventId
                }
            });
            Assert.True(eventResponseYesterday.Success);
            var eventYesterday = eventResponseYesterday.Data;

            // Отправим событие о запуске сегодня
            var eventRequestToday = new SendEventRequest()
            {
                Token = account.GetCoreToken(),
                Data = new SendEventData()
                {
                    ComponentId = component.Info.Id,
                    StartDate = DateTime.Now,
                    TypeSystemName = eventType.SystemName,
                    Category = EventCategory.ComponentEvent,
                    Version = "1.2.3.4",
                    JoinInterval = TimeSpan.FromSeconds(0).TotalSeconds
                }
            };
            var sendEventResponseToday = dispatcher.SendEvent(eventRequestToday);
            Assert.True(sendEventResponseToday.Success);
            var eventResponseToday = dispatcher.GetEventById(new GetEventByIdRequest()
            {
                Token = account.GetCoreToken(),
                Data = new GetEventByIdRequestData()
                {
                    EventId = sendEventResponseToday.Data.EventId
                }
            });
            Assert.True(eventResponseToday.Success);
            var eventToday = eventResponseToday.Data;

            // Проверим отчёт без фильтров
            using (var controller = new ReportsController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Starts(component.Type.Info.Id, fromDate: GuiHelper.GetUrlDateTimeString(yesterdayDate));
                var model = (StartsReportModel)result.Model;

                Assert.Null(model.Error);

                var row = model.Items.SingleOrDefault(t => t.ComponentId == component.Info.Id);
                Assert.NotNull(row);

                Assert.Equal(component.Info.DisplayName, row.ComponentDisplayName);
                Assert.Equal(component.SystemName, row.ComponentSystemName);
                Assert.Equal(TestHelper.GetRoundDateTime(component.Info.CreatedDate), TestHelper.GetRoundDateTime(row.CreateDate));
                Assert.Equal(component.Version, row.Version);
                Assert.Equal(TestHelper.GetRoundDateTime(eventYesterday.StartDate), TestHelper.GetRoundDateTime(row.FirstStart.Value));
                Assert.Equal(eventYesterday.Id, row.FirstStartId);
                Assert.Equal(TestHelper.GetRoundDateTime(eventToday.StartDate), TestHelper.GetRoundDateTime(row.LastStart.Value));
                Assert.Equal(eventToday.Id, row.LastStartId);
                Assert.Equal(2, row.Count);

                var statYesterday = model.Graph.SingleOrDefault(t => t.Date == eventYesterday.StartDate.Date);
                Assert.NotNull(statYesterday);
                Assert.Equal(1, statYesterday.Count);

                var statToday = model.Graph.SingleOrDefault(t => t.Date == eventToday.StartDate.Date);
                Assert.NotNull(statToday);
                Assert.Equal(1, statToday.Count);

                Assert.Equal(2, model.Total);
            }

            // Проверим отчёт с фильтром за вчера
            using (var controller = new ReportsController(account.Id, user.Id))
            {
                var fromDate = eventYesterday.StartDate.Date.ToString(DateTimeFormat.UrlFormat);
                var toDate = eventYesterday.StartDate.Date.AddDays(1).ToString(DateTimeFormat.UrlFormat);
                var result = (ViewResultBase)controller.Starts(component.Type.Info.Id, component.Info.Id, fromDate, toDate);
                var model = (StartsReportModel)result.Model;

                Assert.Null(model.Error);

                var row = model.Items.SingleOrDefault(t => t.ComponentId == component.Info.Id);
                Assert.NotNull(row);

                Assert.Equal(component.Info.DisplayName, row.ComponentDisplayName);
                Assert.Equal(component.SystemName, row.ComponentSystemName);
                Assert.Equal(TestHelper.GetRoundDateTime(component.Info.CreatedDate), TestHelper.GetRoundDateTime(row.CreateDate));
                Assert.Equal(component.Version, row.Version);
                Assert.Equal(TestHelper.GetRoundDateTime(eventYesterday.StartDate), TestHelper.GetRoundDateTime(row.FirstStart.Value));
                Assert.Equal(eventYesterday.Id, row.FirstStartId);
                Assert.Equal(TestHelper.GetRoundDateTime(eventYesterday.StartDate), TestHelper.GetRoundDateTime(row.LastStart.Value));
                Assert.Equal(eventYesterday.Id, row.LastStartId);
                Assert.Equal(1, row.Count);
            }

        }
    }
}
