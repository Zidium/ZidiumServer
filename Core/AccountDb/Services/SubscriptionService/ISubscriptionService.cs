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
        SubscriptionForRead CreateSubscription(CreateSubscriptionRequestData requestData);

        /// <summary>
        /// Изменение параметров подписки
        /// </summary>
        /// <param name="requestData"></param>
        /// 
        /// <returns></returns>
        void UpdateSubscription(UpdateSubscriptionRequestData requestData);


        /// <summary>
        /// Создание подписок по умолчанию для нового пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// 
        SubscriptionForRead CreateDefaultForUser(Guid userId);

        void SetSubscriptionEnable(SetSubscriptionEnableRequestData requestData);

        void SetSubscriptionDisable(SetSubscriptionDisableRequestData requestData);

        void DeleteSubscription(DeleteSubscriptionRequestData requestData);

    }
}
