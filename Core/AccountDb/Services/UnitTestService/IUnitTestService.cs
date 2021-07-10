using System;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Storage;
using Zidium.Api.Dto;

namespace Zidium.Core.AccountsDb
{
    public interface IUnitTestService
    {
        IUnitTestCacheReadObject GetOrCreateUnitTest(GetOrCreateUnitTestRequestDataDto data);

        void SetUnitTestNextTime(SetUnitTestNextTimeRequestData data);

        void SetUnitTestNextStepProcessTime(SetUnitTestNextStepProcessTimeRequestData data);

        void UpdateUnitTest(UpdateUnitTestRequestData data);

        void Delete(Guid unitTestId);

        Guid AddPingUnitTest(AddPingUnitTestRequestData data);

        Guid AddHttpUnitTest(AddHttpUnitTestRequestData data);

        /// <summary>
        /// Отправляет результат проверки
        /// </summary>
        /// <param name="data"></param>
        IBulbCacheReadObject SendUnitTestResult(
            SendUnitTestResultRequestDataDto data);

        /// <summary>
        /// Отправляет набор результатов проверок
        /// </summary>
        /// <param name="data"></param>
        void SendUnitTestResults(SendUnitTestResultRequestDataDto[] data);

        /// <summary>
        /// Получает результат проверки
        /// </summary>
        /// <param name="unitTestId"></param>
        IBulbCacheReadObject GetUnitTestResult(Guid unitTestId);

        IBulbCacheReadObject Disable(SetUnitTestDisableRequestDataDto data);

        IBulbCacheReadObject Enable(Guid unitTestId);

        int RecalcUnitTestsResults(int maxCount);

        void UpdateNoSignalColor(IUnitTestCacheReadObject unitTest);

        string GetFullDisplayName(UnitTestForRead unittest);

    }
}
