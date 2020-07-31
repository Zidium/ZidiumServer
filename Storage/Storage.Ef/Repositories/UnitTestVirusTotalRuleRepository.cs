using System;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class UnitTestVirusTotalRuleRepository : IUnitTestVirusTotalRuleRepository
    {
        public UnitTestVirusTotalRuleRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(UnitTestVirusTotalRuleForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.UnitTestVirusTotalRules.Add(new DbUnitTestVirusTotalRule()
                {
                    UnitTestId = entity.UnitTestId,
                    LastRunErrorCode = entity.LastRunErrorCode,
                    Url = entity.Url,
                    NextStep = entity.NextStep,
                    ScanId = entity.ScanId,
                    ScanTime = entity.ScanTime
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(UnitTestVirusTotalRuleForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var rule = DbGetOneById(entity.UnitTestId);

                if (entity.Url.Changed())
                    rule.Url = entity.Url.Get();

                if (entity.NextStep.Changed())
                    rule.NextStep = entity.NextStep.Get();

                if (entity.ScanTime.Changed())
                    rule.ScanTime = entity.ScanTime.Get();

                if (entity.ScanId.Changed())
                    rule.ScanId = entity.ScanId.Get();

                if (entity.LastRunErrorCode.Changed())
                    rule.LastRunErrorCode = entity.LastRunErrorCode.Get();

                contextWrapper.Context.SaveChanges();
            }
        }

        public UnitTestVirusTotalRuleForRead GetOneByUnitTestId(Guid unitTestId)
        {
            return DbToEntity(DbGetOneById(unitTestId));
        }

        public UnitTestVirusTotalRuleForRead GetOneOrNullByUnitTestId(Guid unitTestId)
        {
            return DbToEntity(DbGetOneOrNullById(unitTestId));
        }

        private DbUnitTestVirusTotalRule DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.UnitTestVirusTotalRules.Find(id);
            }
        }

        private DbUnitTestVirusTotalRule DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Правило VirusTotal-проверки {id} не найдено");

            return result;
        }

        private UnitTestVirusTotalRuleForRead DbToEntity(DbUnitTestVirusTotalRule entity)
        {
            if (entity == null)
                return null;

            return new UnitTestVirusTotalRuleForRead(entity.UnitTestId, entity.Url, entity.NextStep, entity.ScanTime,
                entity.ScanId, entity.LastRunErrorCode);
        }
    }
}
