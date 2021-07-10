using System;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class UnitTestSqlRuleRepository : IUnitTestSqlRuleRepository
    {
        public UnitTestSqlRuleRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(UnitTestSqlRuleForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.UnitTestSqlRules.Add(new DbUnitTestSqlRule()
                {
                    UnitTestId = entity.UnitTestId,
                    ConnectionString = entity.ConnectionString,
                    CommandTimeoutMs = entity.CommandTimeoutMs,
                    OpenConnectionTimeoutMs = entity.OpenConnectionTimeoutMs,
                    Provider = entity.Provider,
                    Query = entity.Query
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(UnitTestSqlRuleForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var rule = DbGetOneById(entity.UnitTestId);

                if (entity.Provider.Changed())
                    rule.Provider = entity.Provider.Get();

                if (entity.ConnectionString.Changed())
                    rule.ConnectionString = entity.ConnectionString.Get();

                if (entity.OpenConnectionTimeoutMs.Changed())
                    rule.OpenConnectionTimeoutMs = entity.OpenConnectionTimeoutMs.Get();

                if (entity.CommandTimeoutMs.Changed())
                    rule.CommandTimeoutMs = entity.CommandTimeoutMs.Get();

                if (entity.Query.Changed())
                    rule.Query = entity.Query.Get();

                contextWrapper.Context.SaveChanges();
            }
        }

        public UnitTestSqlRuleForRead GetOneByUnitTestId(Guid unitTestId)
        {
            return DbToEntity(DbGetOneById(unitTestId));
        }

        public UnitTestSqlRuleForRead GetOneOrNullByUnitTestId(Guid unitTestId)
        {
            return DbToEntity(DbGetOneOrNullById(unitTestId));
        }

        private DbUnitTestSqlRule DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.UnitTestSqlRules.Find(id);
            }
        }

        private DbUnitTestSqlRule DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Правило sql-проверки {id} не найдено");

            return result;
        }

        private UnitTestSqlRuleForRead DbToEntity(DbUnitTestSqlRule entity)
        {
            if (entity == null)
                return null;

            return new UnitTestSqlRuleForRead(entity.UnitTestId, entity.Provider, entity.ConnectionString,
                entity.OpenConnectionTimeoutMs, entity.CommandTimeoutMs, entity.Query);
        }
    }
}
