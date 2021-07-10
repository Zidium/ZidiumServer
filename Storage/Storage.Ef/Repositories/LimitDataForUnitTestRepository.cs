using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class LimitDataForUnitTestRepository : ILimitDataForUnitTestRepository
    {
        public LimitDataForUnitTestRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(LimitDataForUnitTestForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.LimitDatasForUnitTests.Add(new DbLimitDataForUnitTest()
                {
                    Id = entity.Id,
                    UnitTestId = entity.UnitTestId,
                    LimitDataId = entity.LimitDataId,
                    ResultsCount = entity.ResultsCount
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Add(LimitDataForUnitTestForAdd[] entities)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                foreach (var entity in entities)
                {
                    contextWrapper.Context.LimitDatasForUnitTests.Add(new DbLimitDataForUnitTest()
                    {
                        Id = entity.Id,
                        UnitTestId = entity.UnitTestId,
                        LimitDataId = entity.LimitDataId,
                        ResultsCount = entity.ResultsCount
                    });
                }

                contextWrapper.Context.SaveChanges();
            }
        }

        public LimitDataForUnitTestForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public LimitDataForUnitTestForRead GetForUnitTest(Guid limitDataId, Guid unitTestId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return DbToEntity(contextWrapper.Context.LimitDatasForUnitTests.AsNoTracking()
                    .FirstOrDefault(t => t.UnitTestId == unitTestId && t.LimitDataId == limitDataId));
            }
        }

        public Tuple<Guid, int>[] GetGroupedByUnitTest(DateTime date, LimitDataType type)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.LimitDatasForUnitTests.AsQueryable()
                    .Include(t => t.LimitData)
                    .Where(t => t.LimitData.BeginDate >= date &&
                                t.LimitData.Type == LimitDataType.Per5Minutes)
                    .GroupBy(t => t.UnitTestId)
                    .Select(t => new { unitTestId = t.Key, ResultsCount = t.Sum(x => x.ResultsCount) })
                    .AsEnumerable()
                    .Select(t => new Tuple<Guid, int>(t.unitTestId, t.ResultsCount))
                    .ToArray();
            }
        }

        private DbLimitDataForUnitTest DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.LimitDatasForUnitTests.Find(id);
            }
        }

        private DbLimitDataForUnitTest DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Данные лимитов для проверки {id} не найдены");

            return result;
        }

        private LimitDataForUnitTestForRead DbToEntity(DbLimitDataForUnitTest entity)
        {
            if (entity == null)
                return null;

            return new LimitDataForUnitTestForRead(entity.Id, entity.LimitDataId, entity.UnitTestId, entity.ResultsCount);
        }
    }
}
