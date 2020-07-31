using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;
using Zidium.Api;
using Zidium.Core.Common.Helpers;
using Zidium.Storage.Ef;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Statuses
{
    public class ComponentStatusTests
    {
        /// <summary>
        /// Тест проверяет влияние дочерних компонентов на родителя
        /// </summary>
        [Fact]
        public void ParentComponentTest()
        {
            // создадим компонент
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var childComponent = account.CreateRandomComponentControl(component);

            // статус у компонента должен быть "Неизвестно"
            var statusResponse = childComponent.GetTotalState(false);
            Assert.True(statusResponse.Success);
            Assert.Equal(MonitoringStatus.Unknown, statusResponse.Data.Status);
            
            // отправим событие - все ОК (длиной в 1 день)
            var eventResponse = childComponent
                .CreateComponentEvent("Тестовое событие - все ОК")
                .SetImportance(EventImportance.Success)
                .SetJoinInterval(TimeSpan.FromDays(1))
                .Send();

            Assert.True(eventResponse.Success);

            // статус у компонента должен стать Success
            statusResponse = childComponent.GetTotalState(false);
            Assert.True(statusResponse.Success);
            Assert.Equal(MonitoringStatus.Success, statusResponse.Data.Status);

            // статус корня должен тоже быть Success
            statusResponse = component.GetTotalState(false);
            Assert.True(statusResponse.Success);
            Assert.Equal(MonitoringStatus.Success, statusResponse.Data.Status);

            // отправим событие - пипец (длиной в 1 день)
            eventResponse = childComponent
                .CreateComponentEvent("Тестовое событие - пипец")
                .SetImportance(EventImportance.Alarm)
                .SetJoinInterval(TimeSpan.FromDays(1))
                .Send();

            Assert.True(eventResponse.Success);

            // статус у компонента должен стать Alarm
            statusResponse = childComponent.GetTotalState(false);
            Assert.True(statusResponse.Success);
            Assert.Equal(MonitoringStatus.Alarm, statusResponse.Data.Status);

            // статус корня должен тоже быть Alarm
            statusResponse = component.GetTotalState(false);
            Assert.True(statusResponse.Success);
            Assert.Equal(MonitoringStatus.Alarm, statusResponse.Data.Status);
        }

        /// <summary>
        /// Тест проверяет влияние обычных событий на статус компонента
        /// </summary>
        [Fact]
        public void CommonEventsTest()
        {
            // создадим компонент
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            // статус у компонента должен быть "Неизвестно"
            var statusResponse = component.GetTotalState(false);
            Assert.True(statusResponse.Success);
            Assert.Equal(MonitoringStatus.Unknown, statusResponse.Data.Status);

            // отправим событие Warning
            var eventResponse = component
                .CreateComponentEvent("Тестовое событие - Warning")
                .SetImportance(EventImportance.Warning)
                .SetJoinInterval(TimeSpan.Zero)
                .Send();

            Assert.True(eventResponse.Success);

            // статус у компонента должен стать Warning
            statusResponse = component.GetTotalState(false);
            Assert.True(statusResponse.Success);
            Assert.Equal(MonitoringStatus.Warning, statusResponse.Data.Status);

            var sendTime = DateTime.Now;
            // отправим событие - Alarm
            eventResponse = component
                .CreateComponentEvent("Тестовое событие - Alarm")
                .SetImportance(EventImportance.Alarm)
                .SetJoinInterval(TimeSpan.FromDays(1))
                .Send();

            Assert.True(eventResponse.Success);

            // статус у компонента должен стать Alarm
            statusResponse = component.GetTotalState(false);
            Assert.True(statusResponse.Success);
            Assert.Equal(MonitoringStatus.Alarm, statusResponse.Data.Status);

            // ждем когда протухнет событие Alarm
            TestHelper.WaitForTime(sendTime.AddMinutes(1));

            // теперь статус должен опять стать Unknown
            statusResponse = component.GetTotalState(true);
            Assert.True(statusResponse.Success);
            Assert.Equal(MonitoringStatus.Unknown, statusResponse.Data.Status);
        }

        /// <summary>
        /// Тест проверяет влияние проверок на статус компонента
        /// </summary>
        [Fact]
        public void UnitTestResultTest()
        {
            // создадим компонент
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();

            // статус у компонента "Неизвестно", т.к. нет событий
            var statusResponse = component.GetTotalState(false);
            Assert.True(statusResponse.Success);
            Assert.Equal(MonitoringStatus.Unknown, statusResponse.Data.Status);

            var now = TestHelper.GetNow();
            var serverNow = component.Client.ToServerTime(now);

            // отправим проверку = Warning
            var uniTest = component.GetOrCreateUnitTestControl("instance");
            var sendResponse = uniTest.SendResult(new SendUnitTestResultData()
            {
                ActualInterval = TimeSpan.FromDays(1),
                Message = "проверка = Warning",
                Result = UnitTestResult.Warning
            });
            Assert.True(sendResponse.Success);

            // статус должен быть Warning
            statusResponse = component.GetTotalState(false);
            Assert.True(statusResponse.Success);
            Assert.Equal(MonitoringStatus.Warning, statusResponse.Data.Status);
            Assert.True(statusResponse.Data.ActualDate.Year > DateTime.Now.Year);

            // теперь отправим ошибку
            var errorResponse = component
                .CreateApplicationError("моя ошибка - тип", "моя ошибка - сообщение")
                .Send();

            Assert.True(errorResponse.Success);

            // статус должен стать alarm
            statusResponse = component.GetTotalState(false);
            Assert.True(statusResponse.Success);
            Assert.Equal(MonitoringStatus.Alarm, statusResponse.Data.Status);
        }

        /// <summary>
        /// Тест проверяет, что протухание статуса проверки изменяет статус компонента
        /// </summary>
        [Fact]
        public void NonActualStatusTest()
        {
            // отправим проверку = Warning
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var unitTest = component.GetOrCreateUnitTestControl("instance");
            var sendResponse = unitTest.SendResult(new SendUnitTestResultData()
            {
                ActualInterval = TimeSpan.FromMinutes(10),
                Result = UnitTestResult.Warning,
                Message = "warning"
            });
            Assert.True(sendResponse.Success);
            var statusResponse = component.GetTotalState(false);
            Assert.Equal(MonitoringStatus.Warning, statusResponse.Data.Status); // статус должен быть Warning

            // отправляем новый статус, который устанавливает время актуальности очень маленьким
            sendResponse = unitTest.SendResult(new SendUnitTestResultData()
            {
                ActualInterval = TimeSpan.FromSeconds(1),
                Result = UnitTestResult.Warning,
                Message = "warning"
            });

            // ждем когда статус протухнет
            Thread.Sleep(1300);

            // статус проверки протух
            var uniTestState = unitTest.GetState().Data;
            Assert.Equal(uniTestState.Status, MonitoringStatus.Alarm);
            Assert.Equal(uniTestState.HasSignal, false);

            // статус компонета красный
            statusResponse = component.GetTotalState(false);
            Assert.Equal(MonitoringStatus.Alarm, statusResponse.Data.Status);
        }
        
        //private Bulb GetExternalStatus(TestAccountInfo account, IComponentControl componentControl)
        //{
        //    using (var accountDbContext = account.GetAccountDbContext())
        //    {
        //        var id = componentControl.Info.Id;
        //        var component = accountDbContext.Components.Single(x => x.Id == id);
        //        var status = component.ExternalStatus;
        //        var r = status.Status;
        //        return status;
        //    }    
        //}

        //private Bulb GetInternalStatus(TestAccountInfo account, IComponentControl componentControl)
        //{
        //    using (var accountDbContext = account.GetAccountDbContext())
        //    {
        //        var id = componentControl.Info.Id;
        //        var component = accountDbContext.Components.Single(x => x.Id == id);
        //        var status = component.InternalStatus;
        //        var r = status.Status;
        //        return status;
        //    }
        //}

        private void CheckStatus(
            TestAccountInfo account, 
            IComponentControl control, 
            MonitoringStatus internalStatus,
            MonitoringStatus externalStatus)
        {
            var client = account.GetClient();
            var internalState = client.ApiService.GetComponentInternalState(control.Info.Id, false).Data;
            Assert.Equal(internalStatus, internalState.Status);
            var totalState = client.ApiService.GetComponentTotalState(control.Info.Id, false).Data;
            Assert.Equal(externalStatus, totalState.Status);
        }

        /// <summary>
        /// Комплексный тест, проверяет передачу статусов по дереву вверх от проверок, событий, детей
        /// </summary>
        [Fact]
        public void ComplexTest()
        {
            var account = TestHelper.GetTestAccount();

            // создадим структуру:
            // - component
            //      - child1
            //          - unitTest1
            //      - child2
            var component = account.CreateRandomComponentControl();
            Assert.False(component.IsFake());
            var child1 = component.GetOrCreateChildComponentControl("type", "child1");
            var unitTest1 = child1.GetOrCreateUnitTestControl("unitTest1");
            var child2 = component.GetOrCreateChildComponentControl("type", "child2");

            // у всех статус неизвестен
            CheckStatus(account, component, MonitoringStatus.Unknown, MonitoringStatus.Unknown);
            CheckStatus(account, child1, MonitoringStatus.Unknown, MonitoringStatus.Unknown);
            CheckStatus(account, child2, MonitoringStatus.Unknown, MonitoringStatus.Unknown);

            // отправим unitTest1
            unitTest1.SendResult(UnitTestResult.Success, TimeSpan.FromDays(1)).Check();
            //account.SaveAllCaches();
            CheckStatus(account, component, MonitoringStatus.Unknown, MonitoringStatus.Success);
            CheckStatus(account, child1, MonitoringStatus.Success, MonitoringStatus.Success);
            CheckStatus(account, child2, MonitoringStatus.Unknown, MonitoringStatus.Unknown);

            // отправим в child2 событие warning 
            child2.CreateComponentEvent("test")
                .SetImportance(EventImportance.Warning)
                .Send()
                .Check();

            //account.SaveAllCaches();
            CheckStatus(account, component, MonitoringStatus.Unknown, MonitoringStatus.Warning);
            CheckStatus(account, child1, MonitoringStatus.Success, MonitoringStatus.Success);
            CheckStatus(account, child2, MonitoringStatus.Warning, MonitoringStatus.Warning);

            // отправим в component событие success 
            component.CreateComponentEvent("test")
                .SetImportance(EventImportance.Success)
                .Send()
                .Check();

            //account.SaveAllCaches();
            CheckStatus(account, component, MonitoringStatus.Success, MonitoringStatus.Warning);
            CheckStatus(account, child1, MonitoringStatus.Success, MonitoringStatus.Success);
            CheckStatus(account, child2, MonitoringStatus.Warning, MonitoringStatus.Warning);

            // сделаем unitTest1 - неактуальным
            unitTest1.SendResult(UnitTestResult.Success, TimeSpan.FromSeconds(1)).Check();
            Thread.Sleep(1000);
            var unitTestState = unitTest1.GetState();
            Assert.Equal(MonitoringStatus.Alarm, unitTestState.Data.Status);
            //account.SaveAllCaches();
            CheckStatus(account, component, MonitoringStatus.Success, MonitoringStatus.Alarm);
            CheckStatus(account, child1, MonitoringStatus.Alarm, MonitoringStatus.Alarm);
            CheckStatus(account, child2, MonitoringStatus.Warning, MonitoringStatus.Warning);

            // отключим unitTest1
            unitTest1.Disable().Check();
            //account.SaveAllCaches();
            CheckStatus(account, component, MonitoringStatus.Success, MonitoringStatus.Warning);
            CheckStatus(account, child1, MonitoringStatus.Unknown, MonitoringStatus.Unknown);
            CheckStatus(account, child2, MonitoringStatus.Warning, MonitoringStatus.Warning);

            // отключим child1
            child1.Disable(null).Check();
            //account.SaveAllCaches();
            CheckStatus(account, component, MonitoringStatus.Success, MonitoringStatus.Warning);
            CheckStatus(account, child1, MonitoringStatus.Disabled, MonitoringStatus.Disabled);
            CheckStatus(account, child2, MonitoringStatus.Warning, MonitoringStatus.Warning);

            // отправим в unitTest1 success
            var sendUnitTestResponse = unitTest1.SendResult(UnitTestResult.Success, TimeSpan.FromDays(1));
            //account.SaveAllCaches();
            Assert.False(sendUnitTestResponse.Success); // проверка выключена, будет ошибка
            CheckStatus(account, component, MonitoringStatus.Success, MonitoringStatus.Warning);
            CheckStatus(account, child1, MonitoringStatus.Disabled, MonitoringStatus.Disabled);
            CheckStatus(account, child2, MonitoringStatus.Warning, MonitoringStatus.Warning);

            // включим unitTest1 и отправим success
            unitTest1.Enable();
            //account.SaveAllCaches();
            sendUnitTestResponse = unitTest1.SendResult(UnitTestResult.Success, TimeSpan.FromDays(1));
            Assert.False(sendUnitTestResponse.Success);// потому что компонент выключен
            CheckStatus(account, component, MonitoringStatus.Success, MonitoringStatus.Warning);
            CheckStatus(account, child1, MonitoringStatus.Disabled, MonitoringStatus.Disabled);
            CheckStatus(account, child2, MonitoringStatus.Warning, MonitoringStatus.Warning);

            // включим child1 и отправим в unitTest1 success
            child1.Enable();
            //account.SaveAllCaches();
            unitTest1.SendResult(UnitTestResult.Success, TimeSpan.FromDays(1)).Check();
            CheckStatus(account, component, MonitoringStatus.Success, MonitoringStatus.Warning);
            CheckStatus(account, child1, MonitoringStatus.Success, MonitoringStatus.Success);
            CheckStatus(account, child2, MonitoringStatus.Warning, MonitoringStatus.Warning);
        }

        private List<DbEvent> GetEvents(TestAccountInfo account, Guid componentId, Guid ownerId, bool ignoreCheck = false)
        {
            account.SaveAllCaches();

            using (var accountDbContext = TestHelper.GetAccountDbContext(account.Id))
            {
                var component = accountDbContext.Components.Single(x => x.Id == componentId);
                using (var storageDbContext = account.GetAccountDbContext())
                {
                    var events = storageDbContext.Events.Where(x => x.OwnerId == ownerId).OrderBy(t => t.CreateDate).ToList();
                    foreach (var eventObj in events)
                    {
                        // самое первое событие не имеет ссылки на статус
                        if (eventObj.StartDate == component.CreatedDate ||
                            eventObj.Importance == Storage.EventImportance.Unknown)
                        {
                            continue;
                        }
                        if (ignoreCheck == false)
                        {
                            if (eventObj.Message == "Нет данных")
                            {
                                continue;
                            }
                            if (eventObj.Message == "Объект выключен")
                            {
                                continue;
                            }
                            if (eventObj.Category == Storage.EventCategory.ComponentExternalStatus)
                            {
                                continue;
                            }
                            Assert.True(eventObj.LastStatusEventId.HasValue);
                            Assert.True(eventObj.StatusEvents.Count > 0);
                        }
                    }
                    return events;
                }
            } 
        }

        private void Check4Events(List<DbEvent> events)
        {
            // должно быть 4 результата = серый, зеленый, красный, зеленый
            Assert.Equal(4, events.Count);

            Assert.True(EventHelper.GetDuration(events[0].StartDate, events[0].ActualDate, DateTime.Now) > TimeSpan.Zero);
            Assert.Equal(events[0].Importance, Storage.EventImportance.Unknown);
            Assert.Equal(events[0].EndDate, events[1].StartDate);

            Assert.True(EventHelper.GetDuration(events[1].StartDate, events[1].ActualDate, DateTime.Now) > TimeSpan.Zero);
            Assert.Equal(events[1].Importance, Storage.EventImportance.Success);
            Assert.Equal(events[1].EndDate, events[2].StartDate);

            Assert.True(EventHelper.GetDuration(events[2].StartDate, events[2].ActualDate, DateTime.Now) > TimeSpan.Zero);
            Assert.Equal(events[2].Importance, Storage.EventImportance.Alarm);
            Assert.Equal(events[2].EndDate, events[3].StartDate);

            //Assert.Equal(events[2].Duration, TimeSpan.Zero);
            Assert.Equal(events[3].Importance, Storage.EventImportance.Success);
        }

        [Fact]
        public void AppErrorStatusEvents()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            Assert.False(component.IsFake());
            account.SaveAllCaches();

            //var events = GetEvents(account.Id, component.Info.Id, component.Info.Id);

            // отправляем событие success
            var successEvent = component.CreateComponentEvent("success")
                .SetImportance(EventImportance.Success);

            var successEventResponse = successEvent.Send();
            successEventResponse.Check();
            account.SaveAllCaches();

            var state = component.GetTotalState(false);
            Assert.Equal(state.Data.Status, MonitoringStatus.Success);

            // отправляем alert
            var sendTime = DateTime.Now;
            var alertEvent = component.CreateComponentEvent("alert")
                .SetImportance(EventImportance.Alarm)
                .Send();

            alertEvent.Check();
            state = component.GetTotalState(false);
            Assert.Equal(state.Data.Status, MonitoringStatus.Alarm);

            // ждем когда ошибка протухнет
            TestHelper.WaitForTime(sendTime.AddMinutes(1));
            TestHelper.WaitTrue(() =>
            {
                state = component.GetTotalState(true);
                return state.Data.Status == MonitoringStatus.Unknown;
            }, TimeSpan.FromMinutes(1));

            var events = GetEvents(account, component.Info.Id, component.Info.Id);
            var externalStatuses = events
                .Where(x => x.Category == Storage.EventCategory.ComponentExternalStatus)
                .OrderBy(x => x.CreateDate)
                .ToList();
        }

        [Fact]
        public void UnitTestsStatusEvents()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            var unitTest = component.GetOrCreateUnitTestControl("test");

            // отправим проверку Success 
            unitTest.SendResult(UnitTestResult.Success, TimeSpan.FromSeconds(1)).Check();

            // ждем когда проверка протухнет
            Thread.Sleep(1000);
            var state = unitTest.GetState();
            Assert.Equal(MonitoringStatus.Alarm, state.Data.Status);

            // отправим проверку Success 
            Thread.Sleep(100);
            unitTest.SendResult(UnitTestResult.Success, TimeSpan.FromDays(1)).Check();

            DbComponent componentObj = null;
            using (var accountDbContext = account.GetAccountDbContext())
            {
                componentObj = accountDbContext.Components.Single(x => x.Id == component.Info.Id);

                // проверим события проверки
                var unitTestEvents = GetEvents(account, component.Info.Id, unitTest.Info.Id);

                var unitTestResults = unitTestEvents
                    .Where(x=>x.Category == Storage.EventCategory.UnitTestStatus)
                    .OrderBy(x=>x.CreateDate)
                    .ToList();

                Check4Events(unitTestResults);

                var unitTestStatuses = unitTestEvents
                    .Where(x=>x.Category == Storage.EventCategory.UnitTestStatus)
                    .OrderBy(x => x.CreateDate)
                    .ToList();

                Check4Events(unitTestStatuses);

                // проверим статус тестов
                var componentEvents = GetEvents(account, componentObj.Id, componentObj.Id);

                var unitTestsStatuses = componentEvents
                    .Where(x => x.Category == Storage.EventCategory.ComponentUnitTestsStatus)
                    .OrderBy(x => x.CreateDate)
                    .ToList();

                Check4Events(unitTestsStatuses);

                // проверим внутренний статус компонента
                var internalStatuses = componentEvents
                    .Where(x => x.Category == Storage.EventCategory.ComponentInternalStatus)
                    .OrderBy(x => x.CreateDate)
                    .ToList();

                Check4Events(internalStatuses);

                // проверим внешний статус компонента
                var externalStatuses = componentEvents
                    .Where(x=>x.Category == Storage.EventCategory.ComponentExternalStatus)
                    .OrderBy(x => x.CreateDate)
                    .ToList();

                // должно быть 3 результата = зеленый, красный, зеленый
                Check4Events(externalStatuses);
            }
        }

        private List<DbEvent> _oldEvents = new List<DbEvent>();

        private List<DbEvent> GetNewEvents(TestAccountInfo account, Guid componentId)
        {
            var events = GetEvents(account, componentId, componentId, true);
            return GetNewEvents(events);
        } 

        private List<DbEvent> GetNewEvents(List<DbEvent> events)
        {
            var newEvents = new List<DbEvent>();
            foreach (var eventObj in events)
            {
                if (_oldEvents.Any(x => x.Id == eventObj.Id))
                {
                    continue;
                }
                newEvents.Add(eventObj);
            }
            _oldEvents.AddRange(newEvents);
            return newEvents;
        }
            
        [Fact]
        public void OfflineComponent1()
        {
            var account = TestHelper.GetTestAccount();
            var parent = account.CreateRandomComponentControl();
            var component = parent.GetOrCreateChildComponentControl("type", "name");
            Assert.False(component.IsFake());

            // при создании компонента получаем 6 событий = 6 колбасок
            var events1 = GetNewEvents(account, component.Info.Id);
            
            Assert.Equal(6, events1.Count);
            foreach (var @event in events1)
            {
                Assert.Null(@event.LastStatusEventId);
                Assert.Equal(EventImportance.Unknown.ToString(), @event.Importance.ToString());
                Assert.Equal("Нет данных", @event.Message);
            }
            
            // отправляем ошибку
            var error = component
                .CreateComponentEvent("test", "error message")
                .SetImportance(EventImportance.Success);

            error.Send().Check();
            account.SaveAllCaches();

            var events2 = GetNewEvents(account, component.Info.Id);
            Assert.Equal(4, events2.Count);
            foreach (var eventObj in events2)
            {
                Assert.NotNull(eventObj.LastStatusEventId);
                Assert.Equal(EventImportance.Success.ToString(), eventObj.Importance.ToString());
                Assert.Equal("error message", eventObj.Message);
            }

            var state = component.GetTotalState(false);
            Assert.Equal(MonitoringStatus.Success, state.Data.Status);

            // выключим компонент
            component.Disable(null);
            state = component.GetTotalState(false);
            Assert.Equal(MonitoringStatus.Disabled, state.Data.Status);

            // отправляем событие в выключенный компонент
            var response = component.CreateApplicationError("error")
                .SetImportance(EventImportance.Alarm)
                .Send();

            Assert.Equal(ResponseCode.ObjectDisabled, response.Code);

            // для выключеного компонента будет 6 новых статусов выключено
            var events3 = GetNewEvents(account, component.Info.Id);
            Assert.Equal(6, events3.Count);
            foreach (var @event in events3)
            {
                Assert.Null(@event.LastStatusEventId);
                Assert.Equal(EventImportance.Unknown.ToString(), @event.Importance.ToString());
                Assert.Equal("Объект выключен", @event.Message);
            }

            // включим компонент
            component.Enable();
            state = component.GetTotalState(false);
            Assert.Equal(MonitoringStatus.Success, state.Data.Status);

            var events4 = GetNewEvents(account, component.Info.Id);
            Assert.Equal(6, events4.Count);

            // у эти колбасок статус success
            var successEvents = events4
                .Where(x => x.Category == Storage.EventCategory.ComponentExternalStatus
                            || x.Category == Storage.EventCategory.ComponentInternalStatus
                            || x.Category == Storage.EventCategory.ComponentEventsStatus)
                .ToList();
            Assert.Equal(3, successEvents.Count);
            foreach (var successEvent in successEvents)
            {
                Assert.Equal("error message", successEvent.Message);
                Assert.Equal(EventImportance.Success.ToString(), successEvent.Importance.ToString());
            }

            // у эти колбасок статус unknown
            var unknownEvents = events4
                .Where(x => x.Category == Storage.EventCategory.ComponentUnitTestsStatus
                            || x.Category == Storage.EventCategory.ComponentMetricsStatus
                            || x.Category == Storage.EventCategory.ComponentChildsStatus)
                .ToList();
            Assert.Equal(3, unknownEvents.Count);
            foreach (var unknownEvent in unknownEvents)
            {
                Assert.Equal("Нет данных", unknownEvent.Message);
                Assert.Equal(EventImportance.Unknown.ToString(), unknownEvent.Importance.ToString());
            }
        }

        [Fact]
        public void OfflineComponentTest()
        {
            var account = TestHelper.GetTestAccount();
            var component = account.CreateRandomComponentControl();
            Assert.False(component.IsFake());
            account.SaveAllCaches();

            var state = component.GetTotalState(false);
            state.Check();
            Assert.Equal(state.Data.Status, MonitoringStatus.Unknown);
            
            component.CreateApplicationError("Error", "error text")
                .SetImportance(EventImportance.Alarm)
                .Send()
                .Check();

            account.SaveAllCaches();
            CheckStatus(account, component, MonitoringStatus.Alarm, MonitoringStatus.Alarm);

            component.Disable(null);
            account.SaveAllCaches();
            CheckStatus(account, component, MonitoringStatus.Disabled, MonitoringStatus.Disabled);
            state = component.GetTotalState(false);
            state.Check();

            component.Enable();
            account.SaveAllCaches();
            CheckStatus(account, component, MonitoringStatus.Alarm, MonitoringStatus.Alarm);
            state = component.GetTotalState(false);
            state.Check();

            var events = GetEvents(account, component.Info.Id, component.Info.Id);
            
            var statuses = events
                .Where(x => x.Category == Storage.EventCategory.ComponentExternalStatus)
                .OrderBy(x => x.CreateDate)
                .ToList();

            Assert.Equal(4, statuses.Count);

            Assert.Equal(statuses[0].Importance, Storage.EventImportance.Unknown);
            Assert.True(EventHelper.GetDuration(statuses[0].StartDate, statuses[0].ActualDate, DateTime.Now) > TimeSpan.Zero);

            Assert.Equal(statuses[1].Importance, Storage.EventImportance.Alarm);
            Assert.True(EventHelper.GetDuration(statuses[1].StartDate, statuses[1].ActualDate, DateTime.Now) > TimeSpan.Zero);

            Assert.Equal(statuses[2].Importance, Storage.EventImportance.Unknown);
            Assert.True(EventHelper.GetDuration(statuses[2].StartDate, statuses[2].ActualDate, DateTime.Now) > TimeSpan.Zero);

            Assert.Equal(statuses[3].Importance, Storage.EventImportance.Alarm);
        }
    }
}
