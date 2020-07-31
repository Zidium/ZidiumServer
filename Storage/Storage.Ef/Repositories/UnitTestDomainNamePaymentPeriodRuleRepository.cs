using System;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class UnitTestDomainNamePaymentPeriodRuleRepository : IUnitTestDomainNamePaymentPeriodRuleRepository
    {
        public UnitTestDomainNamePaymentPeriodRuleRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(UnitTestDomainNamePaymentPeriodRuleForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.UnitTestDomainNamePaymentPeriodRules.Add(new DbUnitTestDomainNamePaymentPeriodRule()
                {
                    UnitTestId = entity.UnitTestId,
                    AlarmDaysCount = entity.AlarmDaysCount,
                    WarningDaysCount = entity.WarningDaysCount,
                    LastRunErrorCode = entity.LastRunErrorCode,
                    Domain = entity.Domain
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(UnitTestDomainNamePaymentPeriodRuleForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var rule = DbGetOneById(entity.UnitTestId);

                if (entity.Domain.Changed())
                    rule.Domain = entity.Domain.Get();

                if (entity.AlarmDaysCount.Changed())
                    rule.AlarmDaysCount = entity.AlarmDaysCount.Get();

                if (entity.WarningDaysCount.Changed())
                    rule.WarningDaysCount = entity.WarningDaysCount.Get();

                if (entity.LastRunErrorCode.Changed())
                    rule.LastRunErrorCode = entity.LastRunErrorCode.Get();

                contextWrapper.Context.SaveChanges();
            }
        }

        public UnitTestDomainNamePaymentPeriodRuleForRead GetOneByUnitTestId(Guid unitTestId)
        {
            return DbToEntity(DbGetOneById(unitTestId));
        }

        public UnitTestDomainNamePaymentPeriodRuleForRead GetOneOrNullByUnitTestId(Guid unitTestId)
        {
            return DbToEntity(DbGetOneOrNullById(unitTestId));
        }

        private DbUnitTestDomainNamePaymentPeriodRule DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.UnitTestDomainNamePaymentPeriodRules.Find(id);
            }
        }

        private DbUnitTestDomainNamePaymentPeriodRule DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Правило domain-проверки {id} не найдено");

            return result;
        }

        private UnitTestDomainNamePaymentPeriodRuleForRead DbToEntity(DbUnitTestDomainNamePaymentPeriodRule entity)
        {
            if (entity == null)
                return null;

            return new UnitTestDomainNamePaymentPeriodRuleForRead(entity.UnitTestId, entity.Domain, entity.AlarmDaysCount,
                entity.WarningDaysCount, entity.LastRunErrorCode);
        }
    }
}
