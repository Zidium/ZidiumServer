using System;
using Zidium.Core.Api;
using Zidium.Core.Caching;

namespace Zidium.Core.AccountsDb
{
    public interface IUnitTestTypeService
    {
        IUnitTestTypeCacheReadObject GetOrCreateUnitTestType(Guid accountId, GetOrCreateUnitTestTypeRequestData data);

        IUnitTestTypeCacheReadObject UpdateUnitTestType(Guid accountId, UpdateUnitTestTypeRequestData data);

        void DeleteUnitTestType(Guid accountId, Guid unitTestTypeId);
    }
}
