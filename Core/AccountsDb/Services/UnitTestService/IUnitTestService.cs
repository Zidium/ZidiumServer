using System;
using System.Collections.Generic;
using Zidium.Core.Api;
using Zidium.Core.Caching;

namespace Zidium.Core.AccountsDb
{
    public interface IUnitTestService
    {
        IUnitTestCacheReadObject GetOrCreateUnitTest(Guid accountId, GetOrCreateUnitTestRequestData data);

        void SetUnitTestNextTime(Guid accountId, SetUnitTestNextTimeRequestData data);

        void UpdateUnitTest(Guid accountId, UpdateUnitTestRequestData data);

        void Delete(Guid accountId, Guid unitTestId);

        UnitTest AddPingUnitTest(Guid accountId, AddPingUnitTestRequestData data);

        UnitTest AddHttpUnitTest(Guid accountId, AddHttpUnitTestRequestData data);

        /// <summary>
        /// Отправляет результат проверки
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        IBulbCacheReadObject SendUnitTestResult(
            Guid accountId,  
            SendUnitTestResultRequestData data);

        /// <summary>
        /// Отправляет набор результатов проверок
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="data"></param>
        void SendUnitTestResults(Guid accountId,
            SendUnitTestResultRequestData[] data);

        /// <summary>
        /// Получает результат проверки
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="unitTestId"></param>
        /// <returns></returns>
        IBulbCacheReadObject GetUnitTestResult(Guid accountId, Guid unitTestId);

        /// <summary>
        /// Проверка лимита на количество http-проверок без баннера
        /// Если лимит достигнут, то проверка отключается
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="unitTestId"></param>
        /// <param name="hasBanner"></param>
        /// <returns></returns>
        bool SendHttpUnitTestBanner(Guid accountId, Guid unitTestId, bool hasBanner);

        IBulbCacheReadObject Disable(Guid accountId, SetUnitTestDisableRequestData data);

        IBulbCacheReadObject Enable(Guid accountId, Guid unitTestId);

        int RecalcUnitTestsResults(Guid accountId, int maxCount);
    }
}
