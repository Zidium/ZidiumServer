using System;
using System.Linq;
using System.Threading;
using NLog;
using Zidium.Agent.AgentTasks.Notifications;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Xunit;
using Zidium.Storage;
using Zidium.Storage.Ef;
using Zidium.TestTools;

namespace Zidium.Core.Tests.AgentTests
{
    public class CreateNotificationTests : BaseTest
    {
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
            var user1 = TestHelper.CreateTestUser(account.Id, displayName: "user1");
            var user2 = TestHelper.CreateTestUser(account.Id, displayName: "user2");
            var user3 = TestHelper.CreateTestUser(account.Id, displayName: "user3");
            var users = new[] { user1, user2, user3 };

            // настроим подписки:
            // все 3 пользователя имеют подписку по умолчанию (зеленый цвет)
            Guid subscription1Id = Guid.Empty;

            // создадим всем пользователям подписки по умолчанию и установим им зеленый цвет
            foreach (var user in users)
            {
                var subscriptionId = dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
                {
                    UserId = user.Id,
                    Channel = SubscriptionChannel.Email,
                    Object = SubscriptionObject.Default
                }).Data.Id;

                dispatcher.UpdateSubscription(account.Id, new UpdateSubscriptionRequestData()
                {
                    Id = subscriptionId,
                    IsEnabled = true,
                    Importance = EventImportance.Success,
                    NotifyBetterStatus = false
                }).Check();

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
                Channel = SubscriptionChannel.Email,
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
                Channel = SubscriptionChannel.Email,
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
                Channel = SubscriptionChannel.Email,
                IsEnabled = true,
                Object = SubscriptionObject.Component,
                Importance = EventImportance.Alarm,
                NotifyBetterStatus = false
            }).Data.Id;

            // установим компоненту зеленый цвет
            unitTest.SendResult(Zidium.Api.UnitTestResult.Success, TimeSpan.FromDays(1)).Check();
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

            var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Success, component.Info.Id, SubscriptionChannel.Email);
            Assert.Equal(1, notifications.Count);
            var notify1 = notifications.Single(x => x.UserId == user1.Id && x.SubscriptionId == subscription1Id);

            // установим компоненту желтый цвет
            unitTest.SendResult(Zidium.Api.UnitTestResult.Warning, TimeSpan.FromDays(1)).Check();
            account.SaveAllCaches();

            // Запустим обработку
            processor.ProcessAccount(account.Id, component.Info.Id, user1.Id);
            processor.ProcessAccount(account.Id, component.Info.Id, user2.Id);
            processor.ProcessAccount(account.Id, component.Info.Id, user3.Id);

            // Проверим, что появилось 2 уведомления:
            // user1  = получит = подписка по умолчанию
            // user2  = получит = подписка на тип компонента
            // user3  = не получит = подписка на красный компонент

            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Warning, component.Info.Id, SubscriptionChannel.Email);
            //var equal = ReferenceEquals(notifications, TestHelper._Last);
            //var notifications2 = TestHelper._Last;
            //Assert.Equal(2, TestHelper._Last.Count);
            //Assert.Equal(2, notifications2.Count);
            Assert.Equal(2, notifications.Count);
            notify1 = notifications.Single(x => x.UserId == user1.Id && x.SubscriptionId == subscription1Id);
            var notify2 = notifications.Single(x => x.UserId == user2.Id && x.SubscriptionId == subscription2Id);


            // установим компоненту красный цвет
            unitTest.SendResult(Zidium.Api.UnitTestResult.Alarm, TimeSpan.FromDays(1)).Check();
            account.SaveAllCaches();

            // Запустим обработку
            processor.ProcessAccount(account.Id, component.Info.Id, user1.Id);
            processor.ProcessAccount(account.Id, component.Info.Id, user2.Id);
            processor.ProcessAccount(account.Id, component.Info.Id, user3.Id);

            // Проверим, что появилось 3 уведомления:
            // user1  = получит = подписка по умолчанию
            // user2  = получит = подписка на тип компонента
            // user3  = получит = подписка на красный компонент

            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Info.Id, SubscriptionChannel.Email);
            Assert.Equal(3, notifications.Count);
            notify1 = notifications.Single(x => x.UserId == user1.Id && x.SubscriptionId == subscription1Id);
            notify2 = notifications.Single(x => x.UserId == user2.Id && x.SubscriptionId == subscription2Id);
            var notify3 = notifications.Single(x => x.UserId == user3.Id && x.SubscriptionId == subscription3Id);
        }


        /// <summary>
        /// Проверяем, что подписка на тип компонента работает для каждого канала
        /// </summary>
        [Theory]
        [InlineData(SubscriptionChannel.Email)]
        [InlineData(SubscriptionChannel.Telegram)]
        [InlineData(SubscriptionChannel.VKontakte)]
        public void NotificationByChannelTest(SubscriptionChannel channel)
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
                Channel = channel,
                Importance = EventImportance.Alarm,
                IsEnabled = true
            });
            subscriptionResponse.Check();

            // Отправим красное событие
            var eventType = TestHelper.GetTestEventType(account.Id);
            var eventResponse = dispatcher.SendEvent(account.Id, new SendEventData()
            {
                TypeSystemName = eventType.SystemName,
                ComponentId = component.Id,
                Category = EventCategory.ApplicationError,
                Importance = EventImportance.Alarm,
            });
            Assert.True(eventResponse.Success);
            account.SaveAllCaches();

            // Запустим обработку
            var processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Проверим, что появилось уведомление с правильными параметрами
            using (var context = account.GetAccountDbContext())
            {
                // Должно быть одно уведомление
                var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, channel);
                Assert.Equal(1, notifications.Count);

                // Проверим параметры
                var notification = notifications[0];

                Assert.NotNull(notification);
                Assert.Equal(channel, notification.Type);

                // Проверим, что уведомление создано по событию красного статуса
                var eventObj = context.Events.Find(notification.EventId);

                Assert.NotNull(eventObj);
                Assert.Equal(EventCategory.ComponentExternalStatus, eventObj.Category);
                Assert.Equal(EventImportance.Alarm, eventObj.Importance);
                Assert.Equal(component.Id, eventObj.OwnerId);
            }

            // Запустим обработку ещё раз
            account.SaveAllCaches();
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Нового уведомления не должно появиться
            {
                var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, channel);
                Assert.Equal(0, notifications.Count);
            }
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
            var eventResponse = dispatcher.SendEvent(account.Id, new Api.SendEventData()
            {
                TypeSystemName = eventType.SystemName,
                ComponentId = component.Id,
                Category = EventCategory.ApplicationError,
                Importance = EventImportance.Alarm,
            });
            Assert.Equal(Zidium.Api.ResponseCode.ObjectDisabled, eventResponse.Code);
            account.SaveAllCaches();

            // Запустим обработку
            var processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Уведомление не должно появиться
            {
                var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, SubscriptionChannel.Email);
                Assert.Equal(0, notifications.Count);
            }

            // Включим компонент
            dispatcher.SetComponentEnable(account.Id, component.Id);
            account.SaveAllCaches();

            // Запустим обработку ещё раз
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Нового уведомления не должно появиться
            {
                var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, SubscriptionChannel.Email);
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
            var eventResponse = dispatcher.SendEvent(account.Id, new Api.SendEventData()
            {
                TypeSystemName = eventType.SystemName,
                ComponentId = component.Id,
                Category = EventCategory.ApplicationError,
                Importance = EventImportance.Alarm,
            });
            eventResponse.Check();
            account.SaveAllCaches();

            // Запустим обработку
            var processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Нового уведомления не должно появиться
            {
                var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, SubscriptionChannel.Email);
                Assert.Equal(0, notifications.Count);
            }
        }

        /// <summary>
        /// Проверим условие по минимальной длительности статуса
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

            var now = dispatcher.GetServerTime().Data.Date;

            // Запустим обработку
            var processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.SetNow(now);
            processor.ProcessAccount(account.Id, component.Id, user.Id);
            Assert.Null(processor.DbProcessor.FirstException);

            // Нового уведомления не должно появиться
            {
                var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, SubscriptionChannel.Email);
                Assert.Equal(0, notifications.Count);
            }

            // Отправим красное событие 1 раз
            var eventType = TestHelper.GetTestEventType(account.Id);
            var eventResponse = dispatcher.SendEvent(account.Id, new Api.SendEventData()
            {
                TypeSystemName = eventType.SystemName,
                ComponentId = component.Id,
                Category = EventCategory.ApplicationError,
                Importance = EventImportance.Alarm,
            });
            Assert.True(eventResponse.Success);
            account.SaveAllCaches();

            now = dispatcher.GetServerTime().Data.Date;

            // Запустим обработку через 9 секунд
            processor.SetNow(now.AddSeconds(9));
            processor.ProcessAccount(account.Id, component.Id, user.Id);
            Assert.Null(processor.DbProcessor.FirstException);

            // Нового уведомления не должно появиться
            {
                var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, SubscriptionChannel.Email);
                Assert.Equal(0, notifications.Count);
            }

            // Запустим обработку через 11 секунд
            processor.SetNow(now.AddSeconds(11));
            account.SaveAllCaches();
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Должно появиться одно уведомление
            {
                var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, SubscriptionChannel.Email);
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
            var eventResponse = dispatcher.SendEvent(account.Id, new Api.SendEventData()
            {
                TypeSystemName = eventType.SystemName,
                ComponentId = component.Id,
                Category = EventCategory.ApplicationError,
                Importance = EventImportance.Alarm,
            });
            Assert.True(eventResponse.Success);
            account.SaveAllCaches();

            // Запустим обработку
            var processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Должно появиться 1 уведомление
            {
                var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, SubscriptionChannel.Email);
                Assert.Equal(1, notifications.Count);
                var notification = notifications.Single();
                Assert.Equal(NotificationReason.NewImportanceStatus, notification.Reason);
            }

            // Запустим обработку ещё раз
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Новых уведомлений быть не должно
            {
                var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, SubscriptionChannel.Email);
                Assert.Equal(0, notifications.Count);
            }

            // Подождём 10 секунд
            Thread.Sleep(11 * 1000);

            // Запустим обработку ещё раз
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Должно появиться новое уведомление
            {
                var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, SubscriptionChannel.Email);
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
            unitTest.SendResult(Zidium.Api.UnitTestResult.Warning, TimeSpan.FromHours(1)).Check();
            account.SaveAllCaches();
            var sendTime = dispatcher.GetServerTime().Data.Date;

            // Запустим обработку
            var processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.SetNow(sendTime);
            processor.ProcessAccount(account.Id, component.Info.Id, user.Id);

            // Должно появиться 0 уведомлений, т.к. ограничение min длительности не выполнилось
            var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Warning, component.Info.Id, SubscriptionChannel.Email);
            Assert.Equal(0, notifications.Count);

            // Запустим обработку ещё раз
            processor.SetNow(sendTime.AddSeconds(10));
            processor.ProcessAccount(account.Id, component.Info.Id, user.Id);
            Assert.Null(processor.DbProcessor.FirstException);

            // Должно быть 1 уведомление, т.к. выполнилось условие min длительности
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Warning, component.Info.Id, SubscriptionChannel.Email);
            Assert.Equal(1, notifications.Count);

            // изменим статус на красный
            unitTest.SendResult(Zidium.Api.UnitTestResult.Alarm, TimeSpan.FromHours(1));

            // и сразу опять изменим на желтый
            unitTest.SendResult(Zidium.Api.UnitTestResult.Warning, TimeSpan.FromHours(1));
            account.SaveAllCaches();
            sendTime = dispatcher.GetServerTime().Data.Date;

            // Запустим обработку ещё раз
            processor.SetNow(sendTime.AddSeconds(10));
            processor.ProcessAccount(account.Id, component.Info.Id, user.Id);

            // Получим 1 новых уведомлений (2 желтых уведомления подряд разрешены!)
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Warning, component.Info.Id, SubscriptionChannel.Email);
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
            unitTest.SendResult(Zidium.Api.UnitTestResult.Alarm, TimeSpan.FromHours(1)).Check();
            account.SaveAllCaches();
            sendTime = dispatcher.GetServerTime().Data.Date;

            // Запустим обработку
            processor.SetNow(sendTime.AddSeconds(10));
            processor.ProcessAccount(account.Id, component.Info.Id, user.Id);

            // Должно появиться 1 уведомление
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Info.Id, SubscriptionChannel.Email);
            Assert.Equal(1, notifications.Count);

            // Запустим обработку ещё раз
            processor.ProcessAccount(account.Id, component.Info.Id, user.Id);

            // Новых уведомлений быть не должно (статус не изменился)
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Info.Id, SubscriptionChannel.Email);
            Assert.Equal(0, notifications.Count);

            // изменим статус на зеленый
            unitTest.SendResult(Zidium.Api.UnitTestResult.Success, TimeSpan.FromHours(1));

            // и сразу опять изменим на красный
            unitTest.SendResult(Zidium.Api.UnitTestResult.Alarm, TimeSpan.FromHours(1));
            account.SaveAllCaches();
            sendTime = dispatcher.GetServerTime().Data.Date;

            // Запустим обработку ещё раз
            processor.SetNow(sendTime.AddSeconds(10));
            processor.ProcessAccount(account.Id, component.Info.Id, user.Id);

            // Получим 2 уведомления (зеленое и красное)
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Info.Id, SubscriptionChannel.Email);
            Assert.Equal(1, notifications.Count);
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Success, component.Info.Id, SubscriptionChannel.Email);
            Assert.Equal(1, notifications.Count);
        }

        /// <summary>
        /// Проверка отправки уведомлений по http
        /// </summary>
        [Fact(Skip = "Http-уведомления временно отключены")]
        public void HttpNotificationTest()
        {
            // Создадим компонент
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var component = account.CreateTestApplicationComponent();
            var componentControl = account.CreateRandomComponentControl();

            // Добавим пользователю http-контакт
            var url = @"http://localhost/url";
            using (var context = account.GetAccountDbContext())
            {
                var contact = new DbUserContact()
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Type = UserContactType.Http,
                    CreateDate = componentControl.Client.ToServerTime(DateTime.Now),
                    Value = url
                };
                context.UserContacts.Add(contact);
                context.SaveChanges();
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
            var eventResponse = dispatcher.SendEvent(account.Id, new Api.SendEventData()
            {
                TypeSystemName = eventType.SystemName,
                ComponentId = component.Id,
                Category = EventCategory.ApplicationError,
                Importance = EventImportance.Alarm
            });
            Assert.True(eventResponse.Success);
            account.SaveAllCaches();

            // Запустим обработку
            var processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Проверим, что появилось уведомление с типом http
            var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, SubscriptionChannel.Http);
            Assert.Equal(1, notifications.Count);

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
            unitTest.SendResult(Zidium.Api.UnitTestResult.Alarm, TimeSpan.FromMinutes(1)).Check();
            account.SaveAllCaches();

            // Запустим обработку
            var processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.ProcessAccount(account.Id, component.Info.Id, user.Id);

            // Проверим, что создалось уведомление
            var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Info.Id, SubscriptionChannel.Email);
            Assert.Equal(1, notifications.Count);
            var notification = notifications.Single();
            Assert.Equal(NotificationReason.NewImportanceStatus, notification.Reason);

            // Отправим зелёную проверку
            unitTest.SendResult(Zidium.Api.UnitTestResult.Success, TimeSpan.FromMinutes(1)).Check();
            account.SaveAllCaches();

            // Проверим, что снова получили уведомление (о том, что стало лучше)
            processor.ProcessAccount(account.Id, component.Info.Id, user.Id);
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Success, component.Info.Id, SubscriptionChannel.Email);
            Assert.Equal(1, notifications.Count);
            notification = notifications.Single();
            Assert.Equal(NotificationReason.BetterStatus, notification.Reason);

            // Отправим жёлтую проверку
            unitTest.SendResult(Zidium.Api.UnitTestResult.Warning, TimeSpan.FromMinutes(1)).Check();
            account.SaveAllCaches();

            // Проверим, что создалось уведомление
            processor.ProcessAccount(account.Id, component.Info.Id, user.Id);
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Warning, component.Info.Id, SubscriptionChannel.Email);
            Assert.Equal(1, notifications.Count);
            notification = notifications.Single();
            Assert.Equal(NotificationReason.NewImportanceStatus, notification.Reason);

            // Отправим зелёную проверку
            unitTest.SendResult(Zidium.Api.UnitTestResult.Success, TimeSpan.FromMinutes(1)).Check();
            account.SaveAllCaches();

            // Проверим, что снова получили уведомление (потому что перешли ИЗ жёлтого статуса)
            processor.ProcessAccount(account.Id, component.Info.Id, user.Id);
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Success, component.Info.Id, SubscriptionChannel.Email);
            Assert.Equal(1, notifications.Count);
            notification = notifications.Single();
            Assert.Equal(NotificationReason.BetterStatus, notification.Reason);

            // Отправим серую проверку
            unitTest.SendResult(Zidium.Api.UnitTestResult.Unknown, TimeSpan.FromMinutes(1)).Check();
            account.SaveAllCaches();

            // Проверим, что уведомления нет
            processor.ProcessAccount(account.Id, component.Info.Id, user.Id);
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Unknown, component.Info.Id, SubscriptionChannel.Email);
            Assert.Equal(0, notifications.Count);

            // Отправим зелёную проверку
            unitTest.SendResult(Zidium.Api.UnitTestResult.Success, TimeSpan.FromMinutes(1)).Check();
            account.SaveAllCaches();

            // Проверим, что уведомления нет
            processor.ProcessAccount(account.Id, component.Info.Id, user.Id);
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Unknown, component.Info.Id, SubscriptionChannel.Email);
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
            var eventResponse = dispatcher.SendEvent(account.Id, new Api.SendEventData()
            {
                TypeSystemName = eventType.SystemName,
                ComponentId = component.Id,
                Category = EventCategory.ApplicationError,
                Importance = EventImportance.Alarm,
            });
            Assert.True(eventResponse.Success);
            account.SaveAllCaches();

            // Запустим обработку
            var processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Проверим, что появилось уведомление
            var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, SubscriptionChannel.Email);
            Assert.Equal(1, notifications.Count);

            // Добавим пользователю новый контакт
            var newEmail = Guid.NewGuid() + "@test.com";
            using (var context = account.GetAccountDbContext())
            {
                var contact = new DbUserContact()
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Type = UserContactType.Email,
                    CreateDate = componentControl.Client.ToServerTime(DateTime.Now),
                    Value = newEmail
                };
                context.UserContacts.Add(contact);
                context.SaveChanges();
            }
            account.SaveAllCaches();

            // Запустим обработку
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Проверим, что новых уведомлений не появилось
            // Старые уведомления не должны приходить на новые контакты
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, SubscriptionChannel.Email);
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
            var sendUnitTestResult = dispatcher.SendUnitTestResult(account.Id, new Api.SendUnitTestResultRequestData()
            {
                UnitTestId = unittest.Id,
                Result = UnitTestResult.Alarm,
                ActualIntervalSeconds = TimeSpan.FromHours(1).TotalSeconds
            });
            Assert.True(sendUnitTestResult.Success);
            account.SaveAllCaches();

            // Запустим обработку
            var processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Проверим, что появилось уведомление
            var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, SubscriptionChannel.Email);
            Assert.Equal(1, notifications.Count);
            var notification = notifications.Single();
            Assert.Equal(NotificationReason.NewImportanceStatus, notification.Reason);

            // Отправим зелёную проверку
            sendUnitTestResult = dispatcher.SendUnitTestResult(account.Id, new Api.SendUnitTestResultRequestData()
            {
                UnitTestId = unittest.Id,
                Result = UnitTestResult.Success,
                ActualIntervalSeconds = TimeSpan.FromHours(1).TotalSeconds
            });
            Assert.True(sendUnitTestResult.Success);
            account.SaveAllCaches();

            // Запустим обработку
            processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Проверим, что уведомлений нет
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Success, component.Id, SubscriptionChannel.Email);
            Assert.Equal(0, notifications.Count);

            // Отправим красную проверку (вторую)
            sendUnitTestResult = dispatcher.SendUnitTestResult(account.Id, new Api.SendUnitTestResultRequestData()
            {
                UnitTestId = unittest.Id,
                Result = UnitTestResult.Alarm,
                ActualIntervalSeconds = TimeSpan.FromHours(1).TotalSeconds
            });
            Assert.True(sendUnitTestResult.Success);
            account.SaveAllCaches();

            // Запустим обработку
            processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Проверим, что ещё раз появилось уведомление
            notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, SubscriptionChannel.Email);
            Assert.Equal(1, notifications.Count);
            notification = notifications.Single();
            Assert.Equal(NotificationReason.NewImportanceStatus, notification.Reason);
        }

        /// <summary>
        /// Проверим отправку, когда текущее время попадает в заданный интервал
        /// </summary>
        [Fact]
        public void SendIntervalInsideTest()
        {
            // Создадим компонент
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var component = account.CreateTestApplicationComponent();

            // Установим часовой пояс клиента UTC +04:00
            var userSettingService = new UserSettingService(TestHelper.GetStorage(account.Id));
            userSettingService.TimeZoneOffsetMinutes(user.Id, 4 * 60);

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
                SendOnlyInInterval = true,
                SendIntervalFromHour = 10,
                SendIntervalFromMinute = 30,
                SendIntervalToHour = 11,
                SendIntervalToMinute = 30
            });
            Assert.True(subscriptionResponse.Success);
            account.SaveAllCaches();

            // Отправим красное событие
            var eventType = TestHelper.GetTestEventType(account.Id);
            var eventResponse = dispatcher.SendEvent(account.Id, new Api.SendEventData()
            {
                TypeSystemName = eventType.SystemName,
                ComponentId = component.Id,
                Category = EventCategory.ApplicationError,
                Importance = EventImportance.Alarm
            });
            Assert.True(eventResponse.Success);
            account.SaveAllCaches();

            // Запустим обработку в 11:00 UTC +04:00
            var now = new DateTimeOffset(2020, 01, 01, 11, 00, 00, TimeSpan.FromHours(4)).UtcDateTime.ToLocalTime();
            var processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.SetNow(now);
            account.SaveAllCaches();
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Должно появиться одно уведомление
            {
                var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, SubscriptionChannel.Email);
                Assert.Equal(1, notifications.Count);
                var notification = notifications.Single();
                Assert.Equal(NotificationReason.NewImportanceStatus, notification.Reason);
            }
        }

        /// <summary>
        /// Проверим отправку, когда текущее время до заданного интервала
        /// </summary>
        [Fact]
        public void SendIntervalBeforeTest()
        {
            // Создадим компонент
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var component = account.CreateTestApplicationComponent();

            // Установим часовой пояс клиента UTC +04:00
            var userSettingService = new UserSettingService(TestHelper.GetStorage(account.Id));
            userSettingService.TimeZoneOffsetMinutes(user.Id, 4 * 60);

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
                SendOnlyInInterval = true,
                SendIntervalFromHour = 10,
                SendIntervalFromMinute = 30,
                SendIntervalToHour = 11,
                SendIntervalToMinute = 30
            });
            Assert.True(subscriptionResponse.Success);
            account.SaveAllCaches();

            // Отправим красное событие
            var eventType = TestHelper.GetTestEventType(account.Id);
            var eventResponse = dispatcher.SendEvent(account.Id, new Api.SendEventData()
            {
                TypeSystemName = eventType.SystemName,
                ComponentId = component.Id,
                Category = EventCategory.ApplicationError,
                Importance = EventImportance.Alarm
            });
            Assert.True(eventResponse.Success);
            account.SaveAllCaches();

            // Запустим обработку в 10:29 UTC +04:00
            var now = new DateTimeOffset(2020, 01, 01, 10, 29, 00, TimeSpan.FromHours(4)).UtcDateTime.ToLocalTime();
            var processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.SetNow(now);
            account.SaveAllCaches();
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Уведомление не должно появиться
            {
                var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, SubscriptionChannel.Email);
                Assert.Equal(0, notifications.Count);
            }
        }

        /// <summary>
        /// Проверим отправку, когда текущее время после заданного интервала
        /// </summary>
        [Fact]
        public void SendIntervalAfterTest()
        {
            // Создадим компонент
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var component = account.CreateTestApplicationComponent();

            // Установим часовой пояс клиента UTC +04:00
            var userSettingService = new UserSettingService(TestHelper.GetStorage(account.Id));
            userSettingService.TimeZoneOffsetMinutes(user.Id, 4 * 60);

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
                SendOnlyInInterval = true,
                SendIntervalFromHour = 10,
                SendIntervalFromMinute = 30,
                SendIntervalToHour = 11,
                SendIntervalToMinute = 30
            });
            Assert.True(subscriptionResponse.Success);
            account.SaveAllCaches();

            // Отправим красное событие
            var eventType = TestHelper.GetTestEventType(account.Id);
            var eventResponse = dispatcher.SendEvent(account.Id, new Api.SendEventData()
            {
                TypeSystemName = eventType.SystemName,
                ComponentId = component.Id,
                Category = EventCategory.ApplicationError,
                Importance = EventImportance.Alarm
            });
            Assert.True(eventResponse.Success);
            account.SaveAllCaches();

            // Запустим обработку в 11:31 UTC +04:00
            var now = new DateTimeOffset(2020, 01, 01, 11, 31, 00, TimeSpan.FromHours(4)).UtcDateTime.ToLocalTime();
            var processor = new CreateNotificationsProcessor(LogManager.GetCurrentClassLogger(), new CancellationToken());
            processor.SetNow(now);
            account.SaveAllCaches();
            processor.ProcessAccount(account.Id, component.Id, user.Id);

            // Уведомление не должно появиться
            {
                var notifications = TestHelper.SendSubscriptionNotifications(account.Id, EventImportance.Alarm, component.Id, SubscriptionChannel.Email);
                Assert.Equal(0, notifications.Count);
            }
        }

    }
}
