using System;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class UnitTestTcpPortRuleRepository : IUnitTestTcpPortRuleRepository
    {
        public UnitTestTcpPortRuleRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(UnitTestTcpPortRuleForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.UnitTestTcpPortRules.Add(new DbUnitTestTcpPortRule()
                {
                    UnitTestId = entity.UnitTestId,
                    LastRunErrorCode = entity.LastRunErrorCode,
                    Host = entity.Host,
                    TimeoutMs = entity.TimeoutMs,
                    Opened = entity.Opened,
                    Port = entity.Port
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(UnitTestTcpPortRuleForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var rule = DbGetOneById(entity.UnitTestId);

                if (entity.Host.Changed())
                    rule.Host = entity.Host.Get();

                if (entity.TimeoutMs.Changed())
                    rule.TimeoutMs = entity.TimeoutMs.Get();

                if (entity.Port.Changed())
                    rule.Port = entity.Port.Get();

                if (entity.Opened.Changed())
                    rule.Opened = entity.Opened.Get();

                if (entity.LastRunErrorCode.Changed())
                    rule.LastRunErrorCode = entity.LastRunErrorCode.Get();

                contextWrapper.Context.SaveChanges();
            }
        }

        public UnitTestTcpPortRuleForRead GetOneByUnitTestId(Guid unitTestId)
        {
            return DbToEntity(DbGetOneById(unitTestId));
        }

        public UnitTestTcpPortRuleForRead GetOneOrNullByUnitTestId(Guid unitTestId)
        {
            return DbToEntity(DbGetOneOrNullById(unitTestId));
        }

        private DbUnitTestTcpPortRule DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.UnitTestTcpPortRules.Find(id);
            }
        }

        private DbUnitTestTcpPortRule DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Правило tcp-проверки {id} не найдено");

            return result;
        }

        private UnitTestTcpPortRuleForRead DbToEntity(DbUnitTestTcpPortRule entity)
        {
            if (entity == null)
                return null;

            return new UnitTestTcpPortRuleForRead(entity.UnitTestId, entity.Host, entity.TimeoutMs, entity.Port,
                entity.Opened, entity.LastRunErrorCode);
        }
    }
}
