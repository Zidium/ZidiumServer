using System;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public interface ILimitDataService
    {
        LimitDataForUnitTestForRead GetOrCreateUnitTestData(Guid limitDataId, Guid unitTestId);
    }
}
