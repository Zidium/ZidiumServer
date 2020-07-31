using System;
using Zidium.Core.Api;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public interface ISubscriptionService
    {
        /// <summary>
        /// Создание новой подписки или получение существующей
        /// </summary>
        /// <returns></returns>
        SubscriptionForRead CreateSubscription(Guid accountId, CreateSubscriptionRequestData requestData);

        /// <summary>
        /// Изменение параметров подписки
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="requestData"></param>
        /// <returns></returns>
        void UpdateSubscription(Guid accountId, UpdateSubscriptionRequestData requestData);


        /// <summary>
        /// Создание подписок по умолчанию для нового пользователя
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="userId"></param>
        SubscriptionForRead CreateDefaultForUser(Guid accountId, Guid userId);

        void SetSubscriptionEnable(Guid accountId, SetSubscriptionEnableRequestData requestData);

        void SetSubscriptionDisable(Guid accountId, SetSubscriptionDisableRequestData requestData);

        void DeleteSubscription(Guid accountId, DeleteSubscriptionRequestData requestData);

    }
}
