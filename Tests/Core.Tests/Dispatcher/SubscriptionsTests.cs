using System.Linq;
using Xunit;
using Zidium.Core.AccountsDb;
using Zidium.TestTools;

namespace Zidium.Core.Tests.Dispatcher
{
    public class SubscriptionsTests
    {
        /// <summary>
        /// Проверим, что создание нового типа компоненрта НЕ создает новых подписок
        /// </summary>
        [Fact]
        public void NewComponentTypeTest()
        {
            // Создадим новый тип компонента
            var account = TestHelper.GetTestAccount();
            var user = TestHelper.CreateTestUser(account.Id);
            var componentType = TestHelper.CreateRandomComponentType(account.Id);

            // Должны появиться подписки на него
            using (var context = account.CreateAccountDbContext())
            {
                var subscriptionRepository = context.GetSubscriptionRepository();
                
                var subscriptions = subscriptionRepository
                    .QueryAll()
                    .Where(t => t.UserId == user.Id)
                    .ToArray();

                Assert.Equal(1, subscriptions.Length);

                // есть только email-подписка по умолчанию 
                var first = subscriptions[0];
                Assert.Equal(SubscriptionObject.Default, first.Object);
                Assert.True(first.IsEnabled);
                Assert.Equal(SubscriptionChannel.Email, first.Channel);
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
            
            using (var context = account.CreateAccountDbContext())
            {
                var subscriptionRepository = context.GetSubscriptionRepository();

                var subscriptions = subscriptionRepository
                    .QueryAll()
                    .Where(t => t.UserId == user.Id)
                    .ToArray();

                Assert.Equal(1, subscriptions.Length);

                // есть только email-подписка по умолчанию 
                var first = subscriptions[0];
                Assert.Equal(SubscriptionObject.Default, first.Object);
                Assert.True(first.IsEnabled);
                Assert.Equal(SubscriptionChannel.Email, first.Channel);
            }
        }
    }
}
