using System;
using System.Linq;
using System.Threading;
using NLog;
using Zidium.Agent.AgentTasks.Notifications;
using Zidium.Api;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Xunit;
using Zidium.TestTools;
using EventImportance = Zidium.Core.Api.EventImportance;
using UnitTestResult = Zidium.Api.UnitTestResult;

namespace Zidium.Core.Tests.AgentTests
{
    public class CreateNotificationTests
    {
        private void SetUserLastName(TestAccountInfo account, User user, string lastName)
        {
            using (var accountDbContext = account.CreateAccountDbContext())
            {
                var fromDb = accountDbContext.Users.Find(user.Id);
                fromDb.LastName = lastName;
                user.LastName = lastName;
                accountDbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Проверим влияние объекта подписки
        /// </summary>
        [Fact]
        public void SubscriptionObjectTest()
        {
            var account = TestHelper.GetTestAccount();
            var dispatcher = TestHelper.GetDispatcherClient();

            // создадим компонент
            var component = account.CreateRandomComponentControl();
            Assert.False(component.IsFake());

            // создадим проверку
            var unitTest = component.GetOrCreateUnitTestControl("test");
            Assert.False(unitTest.IsFake());

            // создадим 3-х пользователей
            var user1 = TestHelper.CreateTestUser(account.Id);
            var user2 = TestHelper.CreateTestUser(account.Id);
            var user3 = TestHelper.CreateTestUser(account.Id);
            var users = new[] { user1, user2, user3 };

            // чтобы проще отлаживать дадим читабельные фамилии
            SetUserLastName(account, user1, "user1");
            SetUserLastName(account, user2, "user2");
            SetUserLastName(account, user3, "user3");

            // настроим подписки:
            // все 3 пользователя имеют подписку по умолчанию (зеленый цвет)
            Guid subscription1Id = Guid.Empty;

            // создадим всем пользователям подписки по умолчанию и установим им зеленый цвет
            foreach (var user in users)
            {
                var subscriptionId = dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
                {
                    UserId = user.Id,
                    Channel = SubscriptionChannel.Sms,
                    IsEnabled = true,
                    Object = SubscriptionObject.Default,
                    Importance = EventImportance.Success,
                    NotifyBetterStatus = false
                }).Data.Id;

                if (user.Id == user1.Id)
                {
                    subscription1Id = subscriptionId;
                }
            }

            // user2 - имеет подписку на тип компонента (желтый цвет)
            var subscription2Id = dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
            {
                UserId = user2.Id,
                ComponentTypeId = component.Type.Info.Id,
                Channel = SubscriptionChannel.Sms,
                IsEnabled = true,
                Object = SubscriptionObject.ComponentType,
                Importance = EventImportance.Warning,
                NotifyBetterStatus = false
            }).Data.Id;

            // user3 - тоже имеет подписку на тип компонента (желтый цвет)
            dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
            {
                UserId = user3.Id,
                ComponentTypeId = component.Type.Info.Id,
                Channel = SubscriptionChannel.Sms,
                IsEnabled = true,
                Object = SubscriptionObject.ComponentType,
                Importance = EventImportance.Warning,
                NotifyBetterStatus = false
            }).Check();

            // user3 - имеет подписку на компонент (красный цвет)
            var subscription3Id = dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
            {
                UserId = user3.Id,
                ComponentId = component.Info.Id,
                Channel = SubscriptionChannel.Sms,
                IsEnabled = true,
                Object = SubscriptionObject.Component,
                Importance = EventImportance.Alarm,
                NotifyBetterStatus = false
            }).Data.Id;

            // установим компоненту зеленый цвет
            unitTest.SendResult(UnitTestResult.Success, TimeSpan.FromDays(1)).Check();
            account.SaveAllCaches();

            // Запустим обработку
            var processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.ProcessAccount(account.Id, component.Info.Id, user1.Id);
            processor.ProcessAccount(account.Id, component.Info.Id, user2.Id);
            processor.ProcessAccount(account.Id, component.Info.Id, user3.Id);

            // Проверим, что появилось 1 уведомление:
            // user1  = подписка по умолчанию
            // user2  = не получит = подписка на желтый тип компонента
            // user3  = не получит = подписка на красный компонент

            var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Success, component.Info.Id, NotificationType.Sms);
            Assert.Equal(1, notifications.Count);
            var notify1 = notifications.Single(x => x.UserId == user1.Id && x.SubscriptionId == subscription1Id);

            // установим компоненту желтый цвет
            unitTest.SendResult(UnitTestResult.Warning, TimeSpan.FromDays(1)).Check();
            account.SaveAllCaches();

            // Запустим обработку
            processor.ProcessAccount(account.Id, component.Info.Id, user1.Id);
            processor.ProcessAccount(account.Id, component.Info.Id, user2.Id);
            processor.ProcessAccount(account.Id, component.Info.Id, user3.Id);

            // Проверим, что появилось 2 уведомления:
            // user1  = получит = подписка по умолчанию
            // user2  = получит = подписка на тип компонента
            // user3  = не получит = подписка на красный компонент

            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Warning, component.Info.Id, NotificationType.Sms);
            //var equal = ReferenceEquals(notifications, TestHelper._Last);
            //var notifications2 = TestHelper._Last;
            //Assert.Equal(2, TestHelper._Last.Count);
            //Assert.Equal(2, notifications2.Count);
            Assert.Equal(2, notifications.Count);
            notify1 = notifications.Single(x => x.UserId == user1.Id && x.SubscriptionId == subscription1Id);
            var notify2 = notifications.Single(x => x.UserId == user2.Id && x.SubscriptionId == subscription2Id);


            // установим компоненту красный цвет
            unitTest.SendResult(UnitTestResult.Alarm, TimeSpan.FromDays(1)).Check();
            account.SaveAllCaches();

            // Запустим обработку
            processor.ProcessAccount(account.Id, component.Info.Id, user1.Id);
            processor.ProcessAccount(account.Id, component.Info.Id, user2.Id);
            processor.ProcessAccount(account.Id, component.Info.Id, user3.Id);

            // Проверим, что появилось 3 уведомления:
            // user1  = получит = подписка по умолчанию
            // user2  = получит = подписка на тип компонента
            // user3  = получит = подписка на красный компонент

            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Info.Id, NotificationType.Sms);
            Assert.Equal(3, notifications.Count);
            notify1 = notifications.Single(x => x.UserId == user1.Id && x.SubscriptionId == subscription1Id);
            notify2 = notifications.Single(x => x.UserId == user2.Id && x.SubscriptionId == subscription2Id);
            var notify3 = notifications.Single(x => x.UserId == user3.Id && x.SubscriptionId == subscription3Id);
        }


        /// <summary>
        /// Проверяем, что подписка на тип компонента работает
        /// </summary>
        [Fact]
        public void NotificationByEmailTest()
        {
            // Создадим компонент
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var component = account.CreateTestApplicationComponent();

            // Подпишемся на красное событие
            var dispatcher = TestHelper.GetDispatcherClient();
            var subscriptionResponse = dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
            {
                UserId = user.Id,
                Object = SubscriptionObject.ComponentType,
                ComponentTypeId = component.Type.Id,
                Channel = SubscriptionChannel.Email,
                Importance = EventImportance.Alarm,
                IsEnabled = true
            });

            // Отправим красное событие
            var eventType = TestHelper.GetTestEventType(account.Id);
            var eventResponse = dispatcher.SendEvent(account.Id, new Core.Api.SendEventData()
            {
                TypeSystemName = eventType.SystemName,
                ComponentId = component.Id,
                Category = Core.Api.EventCategory.ApplicationError,
                Importance = EventImportance.Alarm,
            });
            Assert.True(eventResponse.Success);
            account.SaveAllCaches();

            // Запустим обработку
            var processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Проверим, что появилось уведомление с правильными параметрами
            using (var context = account.CreateAccountDbContext())
            {
                // Должно быть одно уведомление
                var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, NotificationType.Email);
                Assert.Equal(1, notifications.Count);

                // Проверим параметры
                var notification = notifications[0];

                Assert.NotNull(notification);
                Assert.Equal(NotificationType.Email, notification.Type);

                // Проверим, что уведомление создано по событию красного статуса
                var eventRepository = context.GetEventRepository();
                var eventObj = eventRepository.GetByIdOrNull(notification.EventId);

                Assert.NotNull(eventObj);
                Assert.Equal(Core.Api.EventCategory.ComponentExternalStatus, eventObj.Category);
                Assert.Equal(EventImportance.Alarm, eventObj.Importance);
                Assert.Equal(component.Id, eventObj.OwnerId);
            }

            // Запустим обработку ещё раз
            account.SaveAllCaches();
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Нового уведомления не должно появиться
            {
                var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, NotificationType.Email);
                Assert.Equal(0, notifications.Count);
            }
        }

        /// <summary>
        /// Проверяем, что рабоают уведомления по смс для красного цвета
        /// </summary>
        [Fact]
        public void NotificationBySmsTest()
        {
            // Создадим компонент
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var component = account.CreateTestApplicationComponent();

            // Подпишемся на красное событие
            var dispatcher = TestHelper.GetDispatcherClient();
            var subscriptionResponse = dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
            {
                UserId = user.Id,
                Object = SubscriptionObject.ComponentType,
                ComponentTypeId = component.Type.Id,
                Channel = SubscriptionChannel.Sms,
                Importance = EventImportance.Alarm,
                IsEnabled = true
            });
            Assert.True(subscriptionResponse.Success);

            // Отправим красное событие
            var eventType = TestHelper.GetTestEventType(account.Id);
            var eventResponse = dispatcher.SendEvent(account.Id, new Core.Api.SendEventData()
            {
                TypeSystemName = eventType.SystemName,
                ComponentId = component.Id,
                Category = Core.Api.EventCategory.ApplicationError,
                Importance = EventImportance.Alarm,
            });
            Assert.True(eventResponse.Success);
            account.SaveAllCaches();

            // Запустим обработку
            var processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Должно быть одно уведомление
            var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, NotificationType.Sms);
            Assert.Equal(1, notifications.Count);

            // Проверим параметры
            var notification = notifications[0];

            Assert.NotNull(notification);
            Assert.Equal(NotificationType.Sms, notification.Type);

            using (var context = account.CreateAccountDbContext())
            {
                // Проверим, что уведомление создано по событию красного статуса
                var eventRepository = context.GetEventRepository();
                var eventObj = eventRepository.GetByIdOrNull(notification.EventId);

                Assert.NotNull(eventObj);
                Assert.Equal(Core.Api.EventCategory.ComponentExternalStatus, eventObj.Category);
                Assert.Equal(EventImportance.Alarm, eventObj.Importance);
                Assert.Equal(component.Id, eventObj.OwnerId);
            }

            // Запустим обработку ещё раз
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Нового уведомления не должно появиться
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, NotificationType.Sms);
            Assert.Equal(0, notifications.Count);
        }

        /// <summary>
        /// Проверяем, что для выключенного компонента уведомления о красном цвете не отправляются
        /// </summary>
        [Fact]
        public void ForDisabledComponentTest()
        {
            // Создадим компонент
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var component = account.CreateTestApplicationComponent();

            // Подпишемся на красное событие
            var dispatcher = TestHelper.GetDispatcherClient();
            var subscriptionResponse = dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
            {
                UserId = user.Id,
                Object = SubscriptionObject.ComponentType,
                ComponentTypeId = component.Type.Id,
                Channel = SubscriptionChannel.Email,
                IsEnabled = true,
                Importance = EventImportance.Alarm
            });
            Assert.True(subscriptionResponse.Success);

            // Отключим компонент
            dispatcher.SetComponentDisable(account.Id, new SetComponentDisableRequestData()
            {
                ComponentId = component.Id
            });

            // Отправим красное событие
            var eventType = TestHelper.GetTestEventType(account.Id);
            var eventResponse = dispatcher.SendEvent(account.Id, new Core.Api.SendEventData()
            {
                TypeSystemName = eventType.SystemName,
                ComponentId = component.Id,
                Category = Core.Api.EventCategory.ApplicationError,
                Importance = EventImportance.Alarm,
            });
            Assert.Equal(ResponseCode.ObjectDisabled, eventResponse.Code);
            account.SaveAllCaches();

            // Запустим обработку
            var processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Уведомление не должно появиться
            {
                var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, NotificationType.Email);
                Assert.Equal(0, notifications.Count);
            }

            // Включим компонент
            dispatcher.SetComponentEnable(account.Id, component.Id);
            account.SaveAllCaches();

            // Запустим обработку ещё раз
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Нового уведомления не должно появиться
            {
                var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, NotificationType.Email);
                Assert.Equal(0, notifications.Count);
            }
        }

        /// <summary>
        /// Проверяем, что для выключенной подписки уведомления не создаются
        /// </summary>
        [Fact]
        public void ForDisabledSubscriptionTest()
        {
            // Создадим компонент
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var component = account.CreateTestApplicationComponent();

            // Подпишемся на красное событие и отключим подписку
            var dispatcher = TestHelper.GetDispatcherClient();
            var subscriptionResponse = dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
            {
                UserId = user.Id,
                Object = SubscriptionObject.ComponentType,
                ComponentTypeId = component.Type.Id,
                Channel = SubscriptionChannel.Email,
                Importance = EventImportance.Alarm,
                IsEnabled = false
            });
            Assert.True(subscriptionResponse.Success);

            // Отправим красное событие
            var eventType = TestHelper.GetTestEventType(account.Id);
            var eventResponse = dispatcher.SendEvent(account.Id, new Core.Api.SendEventData()
            {
                TypeSystemName = eventType.SystemName,
                ComponentId = component.Id,
                Category = Core.Api.EventCategory.ApplicationError,
                Importance = EventImportance.Alarm,
            });
            eventResponse.Check();
            account.SaveAllCaches();

            // Запустим обработку
            var processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Нового уведомления не должно появиться
            {
                var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, NotificationType.Email);
                Assert.Equal(0, notifications.Count);
            }
        }

        /// <summary>
        /// Проверим фильтр по минимальной длительности статуса
        /// </summary>
        [Fact]
        public void MinDurationTest()
        {
            // Создадим компонент
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var component = account.CreateTestApplicationComponent();

            // Подпишемся на красное событие
            var dispatcher = TestHelper.GetDispatcherClient();
            var subscriptionResponse = dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
            {
                UserId = user.Id,
                Object = SubscriptionObject.ComponentType,
                ComponentTypeId = component.Type.Id,
                Channel = SubscriptionChannel.Email,
                Importance = EventImportance.Alarm,
                IsEnabled = true,
                DurationMinimumInSeconds = 10
            });
            Assert.True(subscriptionResponse.Success);
            account.SaveAllCaches();

            // Запустим обработку
            var processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.ProcessAccount(account.Id, component.Id, user.Id);
            Assert.Null(processor.DbProcessor.FirstException);

            // Нового уведомления не должно появиться
            {
                var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, NotificationType.Email);
                Assert.Equal(0, notifications.Count);
            }

            // Отправим красное событие 1 раз
            var eventType = TestHelper.GetTestEventType(account.Id);
            var eventResponse = dispatcher.SendEvent(account.Id, new Core.Api.SendEventData()
            {
                TypeSystemName = eventType.SystemName,
                ComponentId = component.Id,
                Category = Core.Api.EventCategory.ApplicationError,
                Importance = EventImportance.Alarm,
            });
            Assert.True(eventResponse.Success);
            account.SaveAllCaches();

            // Запустим обработку
            processor.ProcessAccount(account.Id, component.Id, user.Id);
            Assert.Null(processor.DbProcessor.FirstException);

            // Нового уведомления не должно появиться
            {
                var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, NotificationType.Email);
                Assert.Equal(0, notifications.Count);
            }

            // Подождём 11 секунд
            Thread.Sleep(11 * 1000);

            // Запустим обработку
            account.SaveAllCaches();
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Должно появиться одно уведомление
            {
                var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, NotificationType.Email);
                Assert.Equal(1, notifications.Count);
                var notification = notifications.Single();
                Assert.Equal(NotificationReason.NewImportanceStatus, notification.Reason);
            }
        }

        /// <summary>
        /// Проверяем, что работает повторная отправка уведомлений
        /// </summary>
        [Fact]
        public void ReSendIntervalTest()
        {
            // Создадим компонент
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var component = account.CreateTestApplicationComponent();

            // Подпишемся на красное событие
            var dispatcher = TestHelper.GetDispatcherClient();
            var subscriptionResponse = dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
            {
                UserId = user.Id,
                Object = SubscriptionObject.ComponentType,
                ComponentTypeId = component.Type.Id,
                Channel = SubscriptionChannel.Email,
                Importance = EventImportance.Alarm,
                IsEnabled = true,
                ResendTimeInSeconds = 10
            });
            Assert.True(subscriptionResponse.Success);

            // Отправим красное событие
            var eventType = TestHelper.GetTestEventType(account.Id);
            var eventResponse = dispatcher.SendEvent(account.Id, new Core.Api.SendEventData()
            {
                TypeSystemName = eventType.SystemName,
                ComponentId = component.Id,
                Category = Core.Api.EventCategory.ApplicationError,
                Importance = EventImportance.Alarm,
            });
            Assert.True(eventResponse.Success);
            account.SaveAllCaches();

            // Запустим обработку
            var processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Должно появиться 1 уведомление
            {
                var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, NotificationType.Email);
                Assert.Equal(1, notifications.Count);
                var notification = notifications.Single();
                Assert.Equal(NotificationReason.NewImportanceStatus, notification.Reason);
            }

            // Запустим обработку ещё раз
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Новых уведомлений быть не должно
            {
                var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, NotificationType.Email);
                Assert.Equal(0, notifications.Count);
            }

            // Подождём 10 секунд
            Thread.Sleep(11 * 1000);

            // Запустим обработку ещё раз
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Должно появиться новое уведомление
            {
                var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, NotificationType.Email);
                Assert.Equal(1, notifications.Count);
                var notification = notifications.Single();
                Assert.Equal(NotificationReason.Reminder, notification.Reason);
            }
        }

        /// <summary>
        /// Проверим, что будет создаваться два уведомления об одном и том же статусе
        /// </summary>
        [Fact]
        public void SameColorTest()
        {
            // Создадим компонент
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var component = account.CreateRandomComponentControl();

            // Подпишемся на желтый статус и выше
            var dispatcher = TestHelper.GetDispatcherClient();
            var subscriptionResponse = dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
            {
                UserId = user.Id,
                Object = SubscriptionObject.ComponentType,
                ComponentTypeId = component.Type.Info.Id,
                Channel = SubscriptionChannel.Email,
                Importance = EventImportance.Warning,
                IsEnabled = true,
                ResendTimeInSeconds = 0,
                DurationMinimumInSeconds = 9,
                NotifyBetterStatus = true
            });
            var subscriptionId = subscriptionResponse.Data.Id;

            ////////////////////////////////
            // Часть 1 - создаем колбаски:
            // - желтая 10 сек
            // - красная 1 сек
            // - желтая 10 сек
            // Должны получить 1 уведомление, т.к. запрещено 2 одинаковых уведомления подряд
            // Другими словами компоненту один раз стало плохо, а не два
            ////////////////////////////////

            // Отправим жёлтую проверку
            var unitTest = component.GetOrCreateUnitTestControl("test");
            unitTest.SendResult(UnitTestResult.Warning, TimeSpan.FromHours(1)).Check();
            var sendTime = DateTime.Now;
            account.SaveAllCaches();

            // Запустим обработку
            var processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.ProcessAccount(account.Id, component.Info.Id, user.Id);

            // Должно появиться 0 уведомлений, т.к. ограничение min длительности не выполнилось
            var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Warning, component.Info.Id, NotificationType.Email);
            Assert.Equal(0, notifications.Count);

            // Запустим обработку ещё раз
            TestHelper.WaitForTime(sendTime.AddSeconds(10));
            processor.ProcessAccount(account.Id, component.Info.Id, user.Id);
            Assert.Null(processor.DbProcessor.FirstException);

            // Должно быть 1 уведомление, т.к. выполнилось условие min длительности
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Warning, component.Info.Id, NotificationType.Email);
            Assert.Equal(1, notifications.Count);

            // изменим статус на красный
            unitTest.SendResult(UnitTestResult.Alarm, TimeSpan.FromHours(1));

            // и сразу опять изменим на желтый
            unitTest.SendResult(UnitTestResult.Warning, TimeSpan.FromHours(1));
            sendTime = DateTime.Now;
            account.SaveAllCaches();

            // Запустим обработку ещё раз
            TestHelper.WaitForTime(sendTime.AddSeconds(10));
            processor.ProcessAccount(account.Id, component.Info.Id, user.Id);

            // Получим 1 новых уведомлений (2 желтых уведомления подряд разрешены!)
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Warning, component.Info.Id, NotificationType.Email);
            Assert.Equal(1, notifications.Count);

            ////////////////////////////////
            // Часть 2 - создаем кобаски:
            // - красная 10 сек
            // - зеленая 1 сек
            // - красная 10 сек
            // Должны получить 3 уведомления (1-ое по подписке, 2-ое о том что все хорошо, 3-е по подписке)
            // ВАЖНО! Уведомление о зеленом цвете не попадает под фильтр о минимальной длительности
            ////////////////////////////////

            // Отправим красную проверку
            unitTest.SendResult(UnitTestResult.Alarm, TimeSpan.FromHours(1)).Check();
            account.SaveAllCaches();
            sendTime = DateTime.Now;

            // Запустим обработку
            TestHelper.WaitForTime(sendTime.AddSeconds(10));
            processor.ProcessAccount(account.Id, component.Info.Id, user.Id);

            // Должно появиться 1 уведомление
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Info.Id, NotificationType.Email);
            Assert.Equal(1, notifications.Count);

            // Запустим обработку ещё раз
            processor.ProcessAccount(account.Id, component.Info.Id, user.Id);

            // Новых уведомлений быть не должно (статус не изменился)
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Info.Id, NotificationType.Email);
            Assert.Equal(0, notifications.Count);

            // изменим статус на зеленый
            unitTest.SendResult(UnitTestResult.Success, TimeSpan.FromHours(1));

            // и сразу опять изменим на красный
            unitTest.SendResult(UnitTestResult.Alarm, TimeSpan.FromHours(1));
            sendTime = DateTime.Now;
            account.SaveAllCaches();

            // Запустим обработку ещё раз
            TestHelper.WaitForTime(sendTime.AddSeconds(10));
            processor.ProcessAccount(account.Id, component.Info.Id, user.Id);

            // Получим 2 уведомления (зеленое и красное)
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Info.Id, NotificationType.Email);
            Assert.Equal(1, notifications.Count);
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Success, component.Info.Id, NotificationType.Email);
            Assert.Equal(1, notifications.Count);
        }

        /// <summary>
        /// Проверка отправки уведомлений по http
        /// </summary>
        [Fact]
        public void HttpNotificationTest()
        {
            // Создадим компонент
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var component = account.CreateTestApplicationComponent();
            var componentControl = account.CreateRandomComponentControl();

            // Добавим пользователю http-контакт
            var url = @"http://localhost/url";
            using (var context = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var repository = context.GetUserRepository();
                repository.AddContactToUser(user.Id, UserContactType.Http, url, componentControl.Client.ToServerTime(DateTime.Now));
            }

            // Подпишемся на красное событие
            var dispatcher = TestHelper.GetDispatcherClient();
            var subscriptionResponse = dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
            {
                UserId = user.Id,
                Object = SubscriptionObject.ComponentType,
                ComponentTypeId = component.Type.Id,
                Channel = SubscriptionChannel.Http,
                Importance = EventImportance.Alarm,
                IsEnabled = true
            });
            Assert.True(subscriptionResponse.Success);

            // Отправим красное событие
            var eventType = TestHelper.GetTestEventType(account.Id);
            var eventResponse = dispatcher.SendEvent(account.Id, new Core.Api.SendEventData()
            {
                TypeSystemName = eventType.SystemName,
                ComponentId = component.Id,
                Category = Core.Api.EventCategory.ApplicationError,
                Importance = EventImportance.Alarm
            });
            Assert.True(eventResponse.Success);
            account.SaveAllCaches();

            // Запустим обработку
            var processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Проверим, что появилось уведомление с типом http
            var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, NotificationType.Http);
            var notification = notifications[0];
            Assert.NotNull(notification);
            Assert.Equal(url, notification.Address);

            // Проверим, что текст json заполнен
            //Assert.True(!string.IsNullOrEmpty(notification.NotificationHttp.Json));
        }

        [Fact]
        public void PreviousStatusTest()
        {
            // Создадим компонент
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var component = account.CreateRandomComponentControl();

            // Подпишемся на жёлтый статус
            var dispatcher = TestHelper.GetDispatcherClient();
            var subscriptionResponse = dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
            {
                UserId = user.Id,
                Object = SubscriptionObject.ComponentType,
                ComponentTypeId = component.Type.Info.Id,
                Channel = SubscriptionChannel.Email,
                Importance = EventImportance.Warning,
                IsEnabled = true,
                NotifyBetterStatus = true
            });
            var subscriptionId = subscriptionResponse.Data.Id;

            // Отправим красную проверку
            var unitTest = component.GetOrCreateUnitTestControl("test");
            unitTest.SendResult(UnitTestResult.Alarm, TimeSpan.FromMinutes(1)).Check();
            account.SaveAllCaches();

            // Запустим обработку
            var processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.ProcessAccount(account.Id, component.Info.Id, user.Id);

            // Проверим, что создалось уведомление
            var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Info.Id, NotificationType.Email);
            Assert.Equal(1, notifications.Count);
            var notification = notifications.Single();
            Assert.Equal(NotificationReason.NewImportanceStatus, notification.Reason);

            // Отправим зелёную проверку
            unitTest.SendResult(UnitTestResult.Success, TimeSpan.FromMinutes(1)).Check();
            account.SaveAllCaches();

            // Проверим, что снова получили уведомление (о том, что стало лучше)
            processor.ProcessAccount(account.Id, component.Info.Id, user.Id);
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Success, component.Info.Id, NotificationType.Email);
            Assert.Equal(1, notifications.Count);
            notification = notifications.Single();
            Assert.Equal(NotificationReason.BetterStatus, notification.Reason);

            // Отправим жёлтую проверку
            unitTest.SendResult(UnitTestResult.Warning, TimeSpan.FromMinutes(1)).Check();
            account.SaveAllCaches();

            // Проверим, что создалось уведомление
            processor.ProcessAccount(account.Id, component.Info.Id, user.Id);
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Warning, component.Info.Id, NotificationType.Email);
            Assert.Equal(1, notifications.Count);
            notification = notifications.Single();
            Assert.Equal(NotificationReason.NewImportanceStatus, notification.Reason);

            // Отправим зелёную проверку
            unitTest.SendResult(UnitTestResult.Success, TimeSpan.FromMinutes(1)).Check();
            account.SaveAllCaches();

            // Проверим, что снова получили уведомление (потому что перешли ИЗ жёлтого статуса)
            processor.ProcessAccount(account.Id, component.Info.Id, user.Id);
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Success, component.Info.Id, NotificationType.Email);
            Assert.Equal(1, notifications.Count);
            notification = notifications.Single();
            Assert.Equal(NotificationReason.BetterStatus, notification.Reason);

            // Отправим серую проверку
            unitTest.SendResult(UnitTestResult.Unknown, TimeSpan.FromMinutes(1)).Check();
            account.SaveAllCaches();

            // Проверим, что уведомления нет
            processor.ProcessAccount(account.Id, component.Info.Id, user.Id);
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Unknown, component.Info.Id, NotificationType.Email);
            Assert.Equal(0, notifications.Count);

            // Отправим зелёную проверку
            unitTest.SendResult(UnitTestResult.Success, TimeSpan.FromMinutes(1)).Check();
            account.SaveAllCaches();

            // Проверим, что уведомления нет
            processor.ProcessAccount(account.Id, component.Info.Id, user.Id);
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Unknown, component.Info.Id, NotificationType.Email);
            Assert.Equal(0, notifications.Count);
        }

        /// <summary>
        /// Старые уведомления не должны приходить на новые контакты
        /// </summary>
        [Fact]
        public void NoNotificationForNewContactTest()
        {
            // Создадим компонент
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var component = account.CreateTestApplicationComponent();
            var componentControl = account.CreateRandomComponentControl();

            // Подпишемся на красное событие
            var dispatcher = TestHelper.GetDispatcherClient();
            var subscriptionResponse = dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
            {
                UserId = user.Id,
                Object = SubscriptionObject.ComponentType,
                ComponentTypeId = component.Type.Id,
                Channel = SubscriptionChannel.Email,
                Importance = EventImportance.Alarm,
                IsEnabled = true
            });
            Assert.True(subscriptionResponse.Success);

            // Отправим красное событие
            var eventType = TestHelper.GetTestEventType(account.Id);
            var eventResponse = dispatcher.SendEvent(account.Id, new Core.Api.SendEventData()
            {
                TypeSystemName = eventType.SystemName,
                ComponentId = component.Id,
                Category = Core.Api.EventCategory.ApplicationError,
                Importance = EventImportance.Alarm,
            });
            Assert.True(eventResponse.Success);
            account.SaveAllCaches();

            // Запустим обработку
            var processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Проверим, что появилось уведомление
            var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, NotificationType.Email);
            Assert.Equal(1, notifications.Count);

            // Добавим пользователю новый контакт
            var newEmail = Guid.NewGuid() + "@test.com";
            using (var context = AccountDbContext.CreateFromAccountId(account.Id))
            {
                var userRepository = context.GetUserRepository();
                userRepository.AddContactToUser(user.Id, UserContactType.Email, newEmail, componentControl.Client.ToServerTime(DateTime.Now));
            }
            account.SaveAllCaches();

            // Запустим обработку
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Проверим, что новых уведомлений не появилось
            // Старые уведомления не должны приходить на новые контакты
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, NotificationType.Email);
            Assert.Equal(0, notifications.Count);
        }

        /// <summary>
        /// Проверка, что работает галочка в настройках уведомлений "уведомлять когда стало лучше".
        /// Галока выключена, подписка на красный цвет.
        /// Уведомления о 2-ом красном цвете должно приходить, иначе будет только 1 уведомление за всю жизнь
        /// </summary>
        [Fact]
        public void NotifyBetterStatusTest()
        {
            // Создадим компонент
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var component = account.CreateTestApplicationComponent();

            // Подпишемся на красное событие
            var dispatcher = TestHelper.GetDispatcherClient();
            var subscriptionResponse = dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
            {
                UserId = user.Id,
                Object = SubscriptionObject.ComponentType,
                ComponentTypeId = component.Type.Id,
                Channel = SubscriptionChannel.Email,
                Importance = EventImportance.Alarm,
                IsEnabled = true,
                NotifyBetterStatus = false
            });
            Assert.True(subscriptionResponse.Success);

            // Отправим красную проверку
            var unittest = TestHelper.CreateTestUnitTest(account.Id, component.Id);
            var sendUnitTestResult = dispatcher.SendUnitTestResult(account.Id, new Core.Api.SendUnitTestResultRequestData()
            {
                UnitTestId = unittest.Id,
                Result = Core.Api.UnitTestResult.Alarm,
                ActualIntervalSeconds = TimeSpan.FromHours(1).TotalSeconds
            });
            Assert.True(sendUnitTestResult.Success);
            account.SaveAllCaches();

            // Запустим обработку
            var processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Проверим, что появилось уведомление
            var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, NotificationType.Email);
            Assert.Equal(1, notifications.Count);
            var notification = notifications.Single();
            Assert.Equal(NotificationReason.NewImportanceStatus, notification.Reason);

            // Отправим зелёную проверку
            sendUnitTestResult = dispatcher.SendUnitTestResult(account.Id, new Core.Api.SendUnitTestResultRequestData()
            {
                UnitTestId = unittest.Id,
                Result = Core.Api.UnitTestResult.Success,
                ActualIntervalSeconds = TimeSpan.FromHours(1).TotalSeconds
            });
            Assert.True(sendUnitTestResult.Success);
            account.SaveAllCaches();

            // Запустим обработку
            processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Проверим, что уведомлений нет
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Success, component.Id, NotificationType.Email);
            Assert.Equal(0, notifications.Count);

            // Отправим красную проверку (вторую)
            sendUnitTestResult = dispatcher.SendUnitTestResult(account.Id, new Core.Api.SendUnitTestResultRequestData()
            {
                UnitTestId = unittest.Id,
                Result = Core.Api.UnitTestResult.Alarm,
                ActualIntervalSeconds = TimeSpan.FromHours(1).TotalSeconds
            });
            Assert.True(sendUnitTestResult.Success);
            account.SaveAllCaches();

            // Запустим обработку
            processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Проверим, что ещё раз появилось уведомление
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, NotificationType.Email);
            Assert.Equal(1, notifications.Count);
            notification = notifications.Single();
            Assert.Equal(NotificationReason.NewImportanceStatus, notification.Reason);
        }

    }
}
