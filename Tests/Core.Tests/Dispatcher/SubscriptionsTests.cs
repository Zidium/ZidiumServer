using System.Linq;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.Storage;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Dispatcher
{
    public class SubscriptionsTests : BaseTest
    {
        /// <summary>
        /// Проверим, что создание нового типа компонента НЕ создает новых подписок
        /// </summary>
        [Fact]
        public void NewComponentTypeTest()
        {
            // Создадим новый тип компонента
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var componentType = TestHelper.CreateRandomComponentType(account.Id);

            // Должны появиться подписки на него
            using (var context = account.GetAccountDbContext())
            {
                var subscriptions = context.Subscriptions
                    .Where(t => t.UserId == user.Id)
                    .ToArray();

                Assert.NotEmpty(subscriptions);
            }
        }

        /// <summary>
        /// Проверяем, что при создании пользователя создается одна email-подписка по умолчанию 
        /// </summary>
        [Fact]
        public void NewUserTest()
        {
            // Создадим новый тип компонента
            var account = TestHelper.GetTestAccount();
            var componentType = TestHelper.CreateRandomComponentType(account.Id);

            // Создадим нового пользователя
            var user = TestHelper.CreateTestUser(account.Id);
            
            using (var context = account.GetAccountDbContext())
            {
                var subscriptions = context.Subscriptions
                    .Where(t => t.UserId == user.Id)
                    .ToArray();

                Assert.NotEmpty(subscriptions);
            }
        }

        /// <summary>
        /// Проверим настройки подписок по умолчанию
        /// </summary>
        [Fact]
        public void DefaultSettingsTest()
        {
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);

            using (var context = account.GetAccountDbContext())
            {
                var subscriptions = context.Subscriptions
                    .Where(t => t.UserId == user.Id)
                    .ToArray();

                Assert.Equal(3, subscriptions.Length);

                // EMail - Цвет красный, мин длительность 10m, повтор 1d
                var forAll = subscriptions.FirstOrDefault(t => t.Object == SubscriptionObject.Default);
                Assert.NotNull(forAll);
                Assert.True(forAll.IsEnabled);
                Assert.Equal(SubscriptionChannel.Email, forAll.Channel);
                Assert.Equal(EventImportance.Alarm, forAll.Importance);
                Assert.Equal(10 * 60, forAll.DurationMinimumInSeconds);
                Assert.Equal(24 * 60 * 60, forAll.ResendTimeInSeconds);

                // Корень - уведомления отключены
                var forRoot = subscriptions.FirstOrDefault(t => t.Object == SubscriptionObject.ComponentType && t.ComponentTypeId == SystemComponentType.Root.Id);
                Assert.NotNull(forRoot);
                Assert.False(forRoot.IsEnabled);

                // Папки - уведомления отключены
                var forFolder = subscriptions.FirstOrDefault(t => t.Object == SubscriptionObject.ComponentType && t.ComponentTypeId == SystemComponentType.Folder.Id);
                Assert.NotNull(forFolder);
                Assert.False(forFolder.IsEnabled);
            }
        }
    }
}
