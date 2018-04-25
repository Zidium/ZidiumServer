using System;
using System.Collections.Generic;

namespace Zidium.Core.AccountsDb
{
    public interface ILimitDataForUnitTestRepository : IAccountBasedRepository<LimitDataForUnitTest>
    {
        List<Tuple<Guid, int>> GetGroupedByUnitTest(DateTime date);
    }
}
