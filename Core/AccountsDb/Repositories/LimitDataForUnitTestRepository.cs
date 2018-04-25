using System;
using System.Collections.Generic;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    public class LimitDataForUnitTestRepository : AccountBasedRepository<LimitDataForUnitTest>, ILimitDataForUnitTestRepository
    {
        public LimitDataForUnitTestRepository(AccountDbContext context) : base(context) { }

        public LimitDataForUnitTest Add(LimitDataForUnitTest entity)
        {
            throw new NotImplementedException();
        }

        public LimitDataForUnitTest GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public LimitDataForUnitTest GetByIdOrNull(Guid id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<LimitDataForUnitTest> QueryAll()
        {
            throw new NotImplementedException();
        }

        public void Remove(LimitDataForUnitTest entity)
        {
            throw new NotImplementedException();
        }

        public void Remove(Guid id)
        {
            throw new NotImplementedException();
        }

        public List<Tuple<Guid, int>> GetGroupedByUnitTest(DateTime date)
        {
            var query = Context.LimitDatasForUnitTests
                .Include("LimitData")
                .Where(t => t.LimitData.BeginDate >= date &&
                            t.LimitData.Type == LimitDataType.Per5Minutes)
                .GroupBy(t => t.UnitTestId)
                .Select(t => new {unitTestId = t.Key, ResultsCount = t.Sum(x => x.ResultsCount)})
                .ToList();
            return query.Select(t => new Tuple<Guid, int>(t.unitTestId, t.ResultsCount)).ToList();
        }
    }
}
