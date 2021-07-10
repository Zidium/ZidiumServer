using System;
using Zidium.Core.Common;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public class LimitDataService : ILimitDataService
    {
        public LimitDataService(IStorage storage)
        {
            _storage = storage;
        }

        private readonly IStorage _storage;

        public LimitDataForUnitTestForRead GetOrCreateUnitTestData(Guid limitDataId, Guid unitTestId)
        {
            lock (LockObject.ForUnitTestLimitData(limitDataId, unitTestId))
            {
                var result = _storage.LimitDataForUnitTest.GetForUnitTest(limitDataId, unitTestId);
                if (result == null)
                {
                    var resultForAdd = new LimitDataForUnitTestForAdd()
                    {
                        Id = Guid.NewGuid(),
                        LimitDataId = limitDataId,
                        UnitTestId = unitTestId
                    };
                    _storage.LimitDataForUnitTest.Add(resultForAdd);
                    result = _storage.LimitDataForUnitTest.GetOneById(resultForAdd.Id);
                }
                return result;
            }
        }
    }
}
