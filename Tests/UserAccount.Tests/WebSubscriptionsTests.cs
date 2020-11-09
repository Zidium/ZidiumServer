using System;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core.Api;
using Xunit;
using Zidium.Core;
using Zidium.Storage;
using Zidium.TestTools;
using Zidium.UserAccount.Controllers;
using Zidium.UserAccount.Models;
using Zidium.UserAccount.Models.Controls;
using Zidium.UserAccount.Models.Subscriptions;

namespace Zidium.UserAccount.Tests
{
    public class WebSubscriptionsTests : BaseTest
    {
        [Fact]
        public void EditTest()
        {
            // Создадим пользователя
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var dispatcher = TestHelper.GetDispatcherClient();

            var channels = SubscriptionHelper.AvailableSubscriptionChannels;

            foreach (var channel in channels)
            {
                // Получим подписку по умолчанию
                var response = dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
                {
                    UserId = user.Id,
                    Object = SubscriptionObject.Default,
                    Channel = channel
                });
                var defaultSubscription = response.Data;

                // Изменим подписку по умолчанию
                SubscriptionEditModel model;
                using (var controller = new SubscriptionsController(account.Id, user.Id))
                {
                    var result = (ViewResultBase)controller.Edit(defaultSubscription.Id);
                    model = (SubscriptionEditModel)result.Model;
                }

                model.ReturnUrl = "/";
                model.IsEnabled = !model.IsEnabled;
                model.Color = ColorStatusSelectorValue.FromEventImportance(EventImportance.Success);
                model.MinimumDuration = TimeSpan.FromSeconds(10);
                model.ResendTime = TimeSpan.FromSeconds(20);
                model.SendOnlyInInterval = true;
                model.SendIntervalFrom = new Time() {Hour = 10, Minute = 30};
                model.SendIntervalTo = new Time() {Hour = 18, Minute = 50};

                using (var controller = new SubscriptionsController(account.Id, user.Id))
                {
                    controller.Edit(model);
                }

                // Проверим подписку по умолчанию
                var response2 = dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
                {
                    UserId = user.Id,
                    Object = SubscriptionObject.Default,
                    Channel = channel
                });
                var newDefaultSubscription = response2.Data;

                Assert.Equal(model.IsEnabled, newDefaultSubscription.IsEnabled);
                Assert.Equal(EventImportance.Success, newDefaultSubscription.Importance);
                Assert.Equal(10, newDefaultSubscription.DurationMinimumInSeconds);
                Assert.Equal(20, newDefaultSubscription.ResendTimeInSeconds);
                Assert.True(newDefaultSubscription.SendOnlyInInterval);
                Assert.Equal(10, newDefaultSubscription.SendIntervalFromHour);
                Assert.Equal(30, newDefaultSubscription.SendIntervalFromMinute);
                Assert.Equal(18, newDefaultSubscription.SendIntervalToHour);
                Assert.Equal(50, newDefaultSubscription.SendIntervalToMinute);
            }
        }

        [Fact]
        public void ChangeEnableTest()
        {
            // Создадим пользователя
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var dispatcher = TestHelper.GetDispatcherClient();

            // Создадим тип компонента
            var componentType = TestHelper.CreateRandomComponentType(account.Id);

            var channels = SubscriptionHelper.AvailableSubscriptionChannels;

            foreach (var channel in channels)
            {
                // Настроим подписку по умолчанию и подписку на тип
                var response1 = dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
                {
                    UserId = user.Id,
                    Object = SubscriptionObject.Default,
                    Channel = channel
                });
                var data1 = response1.Data;
                var response2 = dispatcher.UpdateSubscription(account.Id, new UpdateSubscriptionRequestData()
                {
                    Id = data1.Id,
                    IsEnabled = true,
                    Importance = EventImportance.Warning
                });
                response2.Check();

                var response3 = dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
                {
                    UserId = user.Id,
                    Object = SubscriptionObject.ComponentType,
                    Channel = channel,
                    ComponentTypeId = componentType.Id
                });
                var data3 = response3.Data;
                var response4 = dispatcher.UpdateSubscription(account.Id, new UpdateSubscriptionRequestData()
                {
                    Id = data3.Id,
                    IsEnabled = true,
                    Importance = EventImportance.Warning
                });
                response4.Check();

                // Выключим обе подписки
                using (var controller = new SubscriptionsController(account.Id, user.Id))
                {
                    controller.Enable(data1.Id + "," + data3.Id, false, user.Id, channel);
                }

                // Проверим, что подписки выключены
                var response5 = dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
                {
                    UserId = user.Id,
                    Object = SubscriptionObject.Default,
                    Channel = channel
                });
                var data5 = response5.Data;
                Assert.False(data5.IsEnabled);

                var response6 = dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
                {
                    UserId = user.Id,
                    Object = SubscriptionObject.ComponentType,
                    ComponentTypeId = componentType.Id,
                    Channel = channel
                });
                var data6 = response6.Data;
                Assert.False(data6.IsEnabled);

                // Включим обе подписки
                using (var controller = new SubscriptionsController(account.Id, user.Id))
                {
                    controller.Enable(data1.Id + "," + data3.Id, true, user.Id, channel);
                }

                // Проверим, что подписки включены
                var response7 = dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
                {
                    UserId = user.Id,
                    Object = SubscriptionObject.Default,
                    Channel = channel
                });
                var data7 = response7.Data;
                Assert.True(data7.IsEnabled);

                var response8 = dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
                {
                    UserId = user.Id,
                    Object = SubscriptionObject.ComponentType,
                    Channel = channel,
                    ComponentTypeId = componentType.Id
                });
                var data8 = response8.Data;
                Assert.True(data8.IsEnabled);
            }
        }

        [Fact]
        public void ChangeColorTest()
        {
            var channels = SubscriptionHelper.AvailableSubscriptionChannels;

            foreach (var channel in channels)
            {
                // Создадим пользователя
                var account = TestHelper.GetTestAccount();
                var user = TestHelper.CreateTestUser(account.Id);
                var dispatcher = TestHelper.GetDispatcherClient();

                // Создадим тип компонента
                var componentType = TestHelper.CreateRandomComponentType(account.Id);

                // Настроим подписку по умолчанию и подписку на тип
                var response1 = dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
                {
                    UserId = user.Id,
                    Object = SubscriptionObject.Default,
                    Channel = channel
                });
                var data1 = response1.Data;
                var response2 = dispatcher.UpdateSubscription(account.Id, new UpdateSubscriptionRequestData()
                {
                    Id = data1.Id,
                    IsEnabled = true,
                    Importance = EventImportance.Warning
                });
                response2.Check();

                var response3 = dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
                {
                    UserId = user.Id,
                    Object = SubscriptionObject.ComponentType,
                    ComponentTypeId = componentType.Id,
                    Channel = channel
                });
                var data3 = response3.Data;
                var response4 = dispatcher.UpdateSubscription(account.Id, new UpdateSubscriptionRequestData()
                {
                    Id = data3.Id,
                    IsEnabled = true,
                    Importance = EventImportance.Warning
                });
                response4.Check();

                // Поменяем цвет подписок на красный
                using (var controller = new SubscriptionsController(account.Id, user.Id))
                {
                    controller.Color(data1.Id + "," + data3.Id, ObjectColor.Red.ToString(), user.Id, channel);
                }

                // Проверим, что цвет подписок стал красным
                var response5 = dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
                {
                    UserId = user.Id,
                    Object = SubscriptionObject.Default,
                    Channel = channel
                });
                var data5 = response5.Data;
                Assert.Equal(EventImportance.Alarm, data5.Importance);

                var response6 = dispatcher.CreateSubscription(account.Id, new CreateSubscriptionRequestData()
                {
                    UserId = user.Id,
                    ComponentTypeId = componentType.Id,
                    Object = SubscriptionObject.ComponentType,
                    Channel = channel
                });
                var data6 = response6.Data;
                Assert.Equal(EventImportance.Alarm, data6.Importance);
            }
        }

        /// <summary>
        /// Тест проверяет, что обычный пользователь не имеет права редактировать чужие подписки
        /// </summary>
        [Fact]
        public void NonAdminTest()
        {
            // Создадим пользователя
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var admin = TestHelper.GetAccountAdminUser(account.Id);

            // Проверим, что обычный пользователь может видеть чужие подписки
            SubscriptionListModel model;
            using (var controller = new SubscriptionsController(account.Id, user.Id))
            {
                // прочитаем подписки админа
                var result = (ViewResultBase)controller.Index(admin.Id);
                model = (SubscriptionListModel)result.Model;
            }
            Assert.Equal(admin.Id, model.UserId);
            Assert.True(model.Subscriptions.Length > 0);
            Assert.True(model.Subscriptions.All(t => t.UserId == admin.Id));

            // откроем подписку админа на редактирование
            var adminSubscription = model.Subscriptions.First();
            SubscriptionEditModel editModel = null;
            using (var controller = new SubscriptionsController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Edit(adminSubscription.Id);
                editModel = (SubscriptionEditModel)result.Model;
            }

            // сохраним изменения в подписке админа
            using (var controller = new SubscriptionsController(account.Id, user.Id))
            {
                editModel.MinimumDuration = TimeSpan.FromSeconds(10);
                var result = (ViewResultBase)controller.Edit(editModel);
            }

            // проверим, что настройки подписки НЕ изменились
            using (var accountDbContext = account.GetAccountDbContext())
            {
                var subscription = accountDbContext.Subscriptions.Find(adminSubscription.Id);
                Assert.Equal(adminSubscription.DurationMinimumInSeconds, subscription.DurationMinimumInSeconds);
            }
        }

        [Fact]
        public void AdminTest()
        {
            // Создадим пользователя
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var admin = TestHelper.GetAccountAdminUser(account.Id);

            // Проверим, что админ может видеть чужие подписки
            SubscriptionListModel model;
            using (var controller = new SubscriptionsController(account.Id, admin.Id))
            {
                // прочитаем подписки пользователя
                var result = (ViewResultBase)controller.Index(user.Id);
                model = (SubscriptionListModel)result.Model;
            }
            Assert.Equal(user.Id, model.UserId);
            Assert.True(model.Subscriptions.Length > 0);
            Assert.True(model.Subscriptions.All(t => t.UserId == user.Id));

            // откроем подписку пользователя на редактирование
            var userSubscription = model.Subscriptions.First();
            SubscriptionEditModel editModel = null;
            using (var controller = new SubscriptionsController(account.Id, user.Id))
            {
                var result = (ViewResultBase)controller.Edit(userSubscription.Id);
                editModel = (SubscriptionEditModel)result.Model;
            }

            // сохраним изменения в подписке пользователя
            using (var controller = new SubscriptionsController(account.Id, admin.Id))
            {
                editModel.MinimumDuration = TimeSpan.FromSeconds(10);
                controller.Edit(editModel);
            }

            // проверим, что настройки подписки пользователя изменились
            using (var accountDbContext = account.GetAccountDbContext())
            {
                var subscription = accountDbContext.Subscriptions.Find(userSubscription.Id);
                Assert.Equal(10, subscription.DurationMinimumInSeconds);
            }
        }
    }
}
