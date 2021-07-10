using System;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Api.Dto;

namespace Zidium.Core.AccountsDb
{
    public interface IUnitTestTypeService
    {
        IUnitTestTypeCacheReadObject GetOrCreateUnitTestType(GetOrCreateUnitTestTypeRequestDataDto data);

        IUnitTestTypeCacheReadObject GetUnitTestTypeById(Guid id);

        void UpdateUnitTestType(UpdateUnitTestTypeRequestData data);

        void DeleteUnitTestType(Guid unitTestTypeId);
    }
}
