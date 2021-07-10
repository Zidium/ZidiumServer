using System;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class UnitTestPingRuleRepository : IUnitTestPingRuleRepository
    {
        public UnitTestPingRuleRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(UnitTestPingRuleForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.UnitTestPingRules.Add(new DbUnitTestPingRule()
                {
                    UnitTestId = entity.UnitTestId,
                    Host = entity.Host,
                    LastRunErrorCode = entity.LastRunErrorCode,
                    TimeoutMs = entity.TimeoutMs
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(UnitTestPingRuleForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var rule = DbGetOneById(entity.UnitTestId);

                if (entity.Host.Changed())
                    rule.Host = entity.Host.Get();

                if (entity.TimeoutMs.Changed())
                    rule.TimeoutMs = entity.TimeoutMs.Get();

                if (entity.LastRunErrorCode.Changed())
                    rule.LastRunErrorCode = entity.LastRunErrorCode.Get();

                contextWrapper.Context.SaveChanges();
            }
        }

        public UnitTestPingRuleForRead GetOneByUnitTestId(Guid unitTestId)
        {
            return DbToEntity(DbGetOneById(unitTestId));
        }

        public UnitTestPingRuleForRead GetOneOrNullByUnitTestId(Guid unitTestId)
        {
            return DbToEntity(DbGetOneOrNullById(unitTestId));
        }

        private DbUnitTestPingRule DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.UnitTestPingRules.Find(id);
            }
        }

        private DbUnitTestPingRule DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Правило ping-проверки {id} не найдено");

            return result;
        }

        private UnitTestPingRuleForRead DbToEntity(DbUnitTestPingRule entity)
        {
            if (entity == null)
                return null;

            return new UnitTestPingRuleForRead(entity.UnitTestId, entity.Host, entity.TimeoutMs, entity.LastRunErrorCode);
        }
    }
}
