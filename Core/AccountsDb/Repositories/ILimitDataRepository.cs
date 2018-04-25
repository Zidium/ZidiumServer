using System;
using System.Collections.Generic;

namespace Zidium.Core.AccountsDb
{
    public interface ILimitDataRepository : IAccountBasedRepository<LimitData>
    {
        void RemoveOld(DateTime date, LimitDataType type);

        List<LimitData> GetByPeriod(DateTime fromDate, DateTime toDate);
    }
}
