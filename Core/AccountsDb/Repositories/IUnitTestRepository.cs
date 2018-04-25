using System;
using System.Collections.Generic;
using System.Linq;
using Zidium.Core.Api;

namespace Zidium.Core.AccountsDb
{
    public interface IUnitTestRepository : IAccountBasedRepository<UnitTest>
    {
        int GetCount();

        UnitTest GetOneOrNull(Guid componentId,
            Guid unitTestTypeId,
            string systemName);

        List<UnitTest> GetForProcessing(Guid typeId, DateTime date);

        IQueryable<UnitTest> QueryForGui(Guid? componentTypeId,
            Guid? componentId,
            Guid? unitTestTypeId,
            List<MonitoringStatus> statuses);

        IQueryable<UnitTest> QueryNonActual();

        IQueryable<UnitTest> QueryAllWithDeleted();
    }
}
