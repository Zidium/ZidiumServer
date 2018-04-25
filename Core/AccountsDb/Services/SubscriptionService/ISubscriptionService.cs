using System;
using Zidium.Core.Api;

namespace Zidium.Core.AccountsDb
{
    public interface ISubscriptionService
    {
        /// <summary>
        /// Создание новой подписки или получение существующей
        /// </summary>
        /// <returns></returns>
        Subscription CreateSubscription(Guid accountId, CreateSubscriptionRequestData requestData);

        /// <summary>
        /// Изменение параметров подписки
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="requestData"></param>
        /// <returns></returns>
        Subscription UpdateSubscription(Guid accountId, UpdateSubscriptionRequestData requestData);


        /// <summary>
        /// Создание подписок по умолчанию для нового пользователя
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="userId"></param>
        Subscription CreateDefaultForUser(Guid accountId, Guid userId);

        /// <summary>
        /// Какие каналы подписок доступны для аккаунта
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        SubscriptionChannel[] GetAvailableChannelsForAccount(Guid accountId);

        Subscription SetSubscriptionEnable(Guid accountId, SetSubscriptionEnableRequestData requestData);

        Subscription SetSubscriptionDisable(Guid accountId, SetSubscriptionDisableRequestData requestData);

        void Remove(Guid accountId, Subscription subscription);
    }
}
