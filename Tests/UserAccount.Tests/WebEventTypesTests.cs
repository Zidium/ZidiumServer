using System;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core.AccountsDb;
using Xunit;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;
using Zidium.TestTools;
using Zidium.UserAccount.Controllers;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Tests
{
    public class WebEventTypesTests : BaseTest
    {
        [Fact]
        public void EventTypesListTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var storage = TestHelper.GetStorage(account.Id);
            var guid = Guid.NewGuid().ToString();

            // Создадим три типа событий
            // ComponentEvent, Info
            var eventType1ForAdd = new EventTypeForAdd()
            {
                Category = EventCategory.ComponentEvent,
                DisplayName = "Тестовый тип события " + DateTime.Now.Ticks + " ComponentEvent, Info",
                ImportanceForNew = EventImportance.Success,
                JoinIntervalSeconds = 0,
                SystemName = "EventType.Test " + guid + " ComponentEvent, Info"
            };
            var eventType1 = new EventTypeService(storage).GetOrCreate(eventType1ForAdd, account.Id);

            // ComponentEvent, Alarm
            var eventType2ForAdd = new EventTypeForAdd()
            {
                Category = EventCategory.ComponentEvent,
                DisplayName = "Тестовый тип события " + DateTime.Now.Ticks + " ComponentEvent, Alarm",
                ImportanceForNew = EventImportance.Alarm,
                JoinIntervalSeconds = 0,
                SystemName = "EventType.Test " + guid + " ComponentEvent, Alarm"
            };
            var eventType2 = new EventTypeService(storage).GetOrCreate(eventType2ForAdd, account.Id);

            // ApplicationError, Alarm
            var eventType3ForAdd = new EventTypeForAdd()
            {
                Category = EventCategory.ApplicationError,
                DisplayName = "Тестовый тип события " + DateTime.Now.Ticks + " ApplicationError, Alarm",
                ImportanceForNew = EventImportance.Alarm,
                JoinIntervalSeconds = 0,
                SystemName = "EventType.Test " + guid + " ApplicationError, Alarm"
            };
            var eventType3 = new EventTypeService(storage).GetOrCreate(eventType3ForAdd, account.Id);

            // Проверим, что на странице без фильтров есть только типы с категорией ComponentEvent
            using (var controller = new EventTypesController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Index(search: guid);
                var model = (EventTypesListModel)result.Model;
                var types = model.EventTypes;
                Assert.True(types.Any(t => t.Id == eventType1.Id));
                Assert.True(types.Any(t => t.Id == eventType2.Id));
                Assert.False(types.Any(t => t.Id == eventType3.Id));
            }

            // Проверим фильтр по категории
            using (var controller = new EventTypesController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Index(category: "ApplicationError", search: guid);
                var model = (EventTypesListModel)result.Model;
                Assert.False(model.EventTypes.Any(t => t.Id == eventType1.Id));
                Assert.False(model.EventTypes.Any(t => t.Id == eventType2.Id));
                Assert.True(model.EventTypes.Any(t => t.Id == eventType3.Id));
            }

            // Проверим фильтр по важности
            using (var controller = new EventTypesController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Index(importance: "Alarm", search: guid);
                var model = (EventTypesListModel)result.Model;
                Assert.False(model.EventTypes.Any(t => t.Id == eventType1.Id));
                Assert.True(model.EventTypes.Any(t => t.Id == eventType2.Id));
                Assert.False(model.EventTypes.Any(t => t.Id == eventType3.Id));
            }

            // Проверим фильтр по названию
            using (var controller = new EventTypesController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Index(search: eventType1.SystemName);
                var model = (EventTypesListModel)result.Model;
                Assert.True(model.EventTypes.Any(t => t.Id == eventType1.Id));
                Assert.False(model.EventTypes.Any(t => t.Id == eventType2.Id));
                Assert.False(model.EventTypes.Any(t => t.Id == eventType3.Id));
            }
            using (var controller = new EventTypesController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Index(category: "ApplicationError", search: eventType3.DisplayName);
                var model = (EventTypesListModel)result.Model;
                Assert.False(model.EventTypes.Any(t => t.Id == eventType1.Id));
                Assert.False(model.EventTypes.Any(t => t.Id == eventType2.Id));
                Assert.True(model.EventTypes.Any(t => t.Id == eventType3.Id));
            }

        }

        [Fact]
        public void EventTypeShowTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var eventType = TestHelper.GetTestEventType(account.Id);
            using (var controller = new EventTypesController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Show(eventType.Id, TimelineInterval.Day);
                var model = (EventTypeShowModel)result.Model;
                Assert.Equal(eventType.Id, model.Id);
                Assert.Equal(eventType.DisplayName, model.DisplayName);
                Assert.Equal(eventType.SystemName, model.SystemName);
                Assert.Equal(eventType.Category, model.Category);
                Assert.Equal(eventType.JoinIntervalSeconds, TimeSpanHelper.GetSeconds(model.JoinInterval));
                Assert.Equal(eventType.OldVersion, model.OldVersion);
                Assert.Equal(eventType.ImportanceForOld, model.ImportanceForOld);
                Assert.Equal(eventType.ImportanceForNew, model.ImportanceForNew);
                Assert.Equal(eventType.IsSystem, model.IsSystem);
            }
        }

        [Fact]
        public void EventTypeAddTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);

            EventTypeEditModel model;
            using (var controller = new EventTypesController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Add();
                model = (EventTypeEditModel)result.Model;
            }
            model.DisplayName = "New test event type " + Guid.NewGuid();
            model.SystemName += model.DisplayName;
            model.Category = EventCategory.ComponentEvent;
            model.ImportanceForNew = EventImportance.Success;
            model.JoinInterval = TimeSpan.FromSeconds(10);
            model.OldVersion = "1.0.0.0";
            model.ImportanceForOld = EventImportance.Alarm;
            model.ImportanceForNew = EventImportance.Warning;
            using (var controller = new EventTypesController(account.Id, user.Id))
            {
                controller.Add(model);
            }

            using (var accountContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var eventType = accountContext.EventTypes.FirstOrDefault(t => t.SystemName == model.SystemName);
                Assert.NotNull(eventType);
                Assert.Equal(model.DisplayName, eventType.DisplayName);
                Assert.Equal(model.SystemName, eventType.SystemName);
                Assert.Equal(model.Category, eventType.Category);
                Assert.Equal(model.JoinInterval, TimeSpanHelper.FromSeconds(eventType.JoinIntervalSeconds));
                Assert.Equal(model.OldVersion, eventType.OldVersion);
                Assert.Equal(model.ImportanceForOld, eventType.ImportanceForOld);
                Assert.Equal(model.ImportanceForNew, eventType.ImportanceForNew);
                Assert.Equal(model.IsSystem, eventType.IsSystem);
            }
        }

        [Fact]
        public void EventTypeEditTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var eventType = TestHelper.GetTestEventType(account.Id);
            EventTypeEditModel model;
            using (var controller = new EventTypesController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Edit(eventType.Id);
                model = (EventTypeEditModel)result.Model;
                Assert.Equal(eventType.Id, model.Id);
                Assert.Equal(eventType.DisplayName, model.DisplayName);
                Assert.Equal(eventType.SystemName, model.SystemName);
                Assert.Equal(eventType.Category, model.Category);
                Assert.Equal(TimeSpanHelper.FromSeconds(eventType.JoinIntervalSeconds), model.JoinInterval);
                Assert.Equal(eventType.OldVersion, model.OldVersion);
                Assert.Equal(eventType.ImportanceForOld, model.ImportanceForOld);
                Assert.Equal(eventType.ImportanceForNew, model.ImportanceForNew);
                Assert.Equal(eventType.IsSystem, model.IsSystem);
            }
            model.DisplayName += "New";
            model.SystemName += "New";
            model.Category = EventCategory.ComponentEvent;
            model.JoinInterval = TimeSpanHelper.FromSeconds(10);
            model.OldVersion = "2.0.0.0";
            model.ImportanceForOld = EventImportance.Alarm;
            model.ImportanceForNew = EventImportance.Warning;
            using (var controller = new EventTypesController(account.Id, user.Id))
            {
                controller.Edit(model);
            }

            using (var accountContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var dbEventType = accountContext.EventTypes.Find(model.Id);
                Assert.NotNull(dbEventType);
                Assert.Equal(model.Id, dbEventType.Id);
                Assert.Equal(model.DisplayName, dbEventType.DisplayName);
                Assert.Equal(model.SystemName, dbEventType.SystemName);
                Assert.Equal(model.Category, dbEventType.Category);
                Assert.Equal(model.JoinInterval, TimeSpanHelper.FromSeconds(dbEventType.JoinIntervalSeconds));
                Assert.Equal(model.OldVersion, dbEventType.OldVersion);
                Assert.Equal(model.ImportanceForOld, dbEventType.ImportanceForOld);
                Assert.Equal(model.ImportanceForNew, dbEventType.ImportanceForNew);
                Assert.Equal(model.IsSystem, dbEventType.IsSystem);
            }
        }

        [Fact]
        public void EventTypeDeleteTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.GetAccountAdminUser(account.Id);
            var storage = TestHelper.GetStorage(account.Id);

            DeleteConfirmationModel model;

            var eventTypeForAdd = new EventTypeForAdd()
            {
                Category = EventCategory.ComponentEvent,
                DisplayName = "Тестовый тип события " + DateTime.Now.Ticks,
                ImportanceForNew = EventImportance.Success,
                JoinIntervalSeconds = 0,
                SystemName = "EventType.Test " + DateTime.Now.Ticks
            };
            var eventType = new EventTypeService(storage).GetOrCreate(eventTypeForAdd, account.Id);

            using (var controller = new EventTypesController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Delete(eventType.Id);
                model = (DeleteConfirmationModel)result.Model;
            }
            using (var controller = new EventTypesController(account.Id, user.Id))
            {
                controller.Delete(model);
            }

            using (var accountContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var dbEventType = accountContext.EventTypes.Find(Guid.Parse(model.Id));
                Assert.NotNull(dbEventType);
                Assert.True(dbEventType.IsDeleted);
            }
        }
    }
}
