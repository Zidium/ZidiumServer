using System;
using Zidium.Core.Api;
using Zidium.Core.Api.Accounts.ChangeApiKey;

namespace Zidium.Core.ConfigDb
{
    public interface IAccountService
    {
        /// <summary>
        /// Получение списка всех аккаунтов
        /// </summary>
        AccountInfo[] GetAccounts(GetAccountsRequestData data);

        /// <summary>
        /// Обновление аккаунта
        /// </summary>
        AccountInfo Update(UpdateAccountRequestData data);

        /// <summary>
        /// Получение аккаунта по системному имени
        /// </summary>
        AccountInfo GetOneOrNullBySystemName(string name);

        /// <summary>
        /// Получение аккаунта по Id, если он есть
        /// </summary>
        AccountInfo GetOneOrNullById(Guid accountId);

        /// <summary>
        /// Получение аккаунта по Id
        /// </summary>
        AccountInfo GetOneById(Guid accountId);

        /// <summary>
        /// Получение системного аккаунта
        /// </summary>
        AccountInfo GetSystemAccount();

        string ChangeApiKey(ChangeApiKeyRequestData data);
    }
}
