using System;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class UnitTestSslCertificateExpirationDateRuleRepository  : IUnitTestSslCertificateExpirationDateRuleRepository
    {
        public UnitTestSslCertificateExpirationDateRuleRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(UnitTestSslCertificateExpirationDateRuleForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.UnitTestSslCertificateExpirationDateRules.Add(new DbUnitTestSslCertificateExpirationDateRule()
                {
                    UnitTestId = entity.UnitTestId,
                    AlarmDaysCount = entity.AlarmDaysCount,
                    WarningDaysCount = entity.WarningDaysCount,
                    LastRunErrorCode = entity.LastRunErrorCode,
                    Url = entity.Url
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(UnitTestSslCertificateExpirationDateRuleForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var rule = DbGetOneById(entity.UnitTestId);

                if (entity.Url.Changed())
                    rule.Url = entity.Url.Get();

                if (entity.AlarmDaysCount.Changed())
                    rule.AlarmDaysCount = entity.AlarmDaysCount.Get();

                if (entity.WarningDaysCount.Changed())
                    rule.WarningDaysCount = entity.WarningDaysCount.Get();

                if (entity.LastRunErrorCode.Changed())
                    rule.LastRunErrorCode = entity.LastRunErrorCode.Get();

                contextWrapper.Context.SaveChanges();
            }
        }

        public UnitTestSslCertificateExpirationDateRuleForRead GetOneByUnitTestId(Guid unitTestId)
        {
            return DbToEntity(DbGetOneById(unitTestId));
        }

        public UnitTestSslCertificateExpirationDateRuleForRead GetOneOrNullByUnitTestId(Guid unitTestId)
        {
            return DbToEntity(DbGetOneOrNullById(unitTestId));
        }

        private DbUnitTestSslCertificateExpirationDateRule DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.UnitTestSslCertificateExpirationDateRules.Find(id);
            }
        }

        private DbUnitTestSslCertificateExpirationDateRule DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Правило ssl-проверки {id} не найдено");

            return result;
        }

        private UnitTestSslCertificateExpirationDateRuleForRead DbToEntity(DbUnitTestSslCertificateExpirationDateRule entity)
        {
            if (entity == null)
                return null;

            return new UnitTestSslCertificateExpirationDateRuleForRead(entity.UnitTestId, entity.Url, entity.AlarmDaysCount,
                entity.WarningDaysCount, entity.LastRunErrorCode);
        }
    }
}
