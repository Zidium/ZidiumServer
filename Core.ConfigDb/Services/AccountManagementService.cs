using System;
using Zidium.Core.Api;
using Zidium.Storage;

namespace Zidium.Core.ConfigDb
{
    public class AccountManagementService : IAccountManagementService
    {
        public Guid RegistrationStep1(string accountName, string email, Guid? userAgentTag)
        {
            throw new NotImplementedException();
        }

        public Guid RegistrationStep2(Guid regId, string companyName, string site, string companyPost)
        {
            throw new NotImplementedException();
        }

        public Guid RegistrationStep3(Guid regId, string firstName, string lastName, string fatherName, string phone)
        {
            throw new NotImplementedException();
        }

        public Guid EndRegistration(string secretKey)
        {
            throw new NotImplementedException();
        }

        public void MakeAccountFree(Guid accountId)
        {
            throw new NotImplementedException();
        }

        public void MakeAccountPaidAndSetLimits(Guid accountId, TariffConfigurationInfo data)
        {
            throw new NotImplementedException();
        }

        public void ChangeAccountType(Guid accountId, AccountType accountType)
        {
            throw new NotImplementedException();
        }

        public void ValidateAccountName(string name)
        {
            throw new NotImplementedException();
        }

        public Guid CreateAccount(CreateAccountData data)
        {
            throw new NotImplementedException();
        }

        public void SetAccountLimits(Guid accountId, AccountTotalLimitsDataInfo limits, TariffLimitType type)
        {
            var accountStorageFactory = DependencyInjection.GetServicePersistent<IAccountStorageFactory>();
            var accountStorage = accountStorageFactory.GetStorageByAccountId(accountId);

            var currentTariff = accountStorage.TariffLimits.GetBaseTariffLimit(type);

            var currentTariffForUpdate = currentTariff.GetForUpdate();
            currentTariffForUpdate.EventsRequestsPerDay.Set(limits.EventRequestsPerDay);
            currentTariffForUpdate.EventsMaxDays.Set(limits.EventsMaxDays);
            currentTariffForUpdate.LogMaxDays.Set(limits.LogMaxDays);
            currentTariffForUpdate.LogSizePerDay.Set(limits.LogSizePerDay);
            currentTariffForUpdate.ComponentsMax.Set(limits.ComponentsMax);
            currentTariffForUpdate.ComponentTypesMax.Set(limits.ComponentTypesMax);
            currentTariffForUpdate.UnitTestTypesMax.Set(limits.UnitTestTypesMax);
            currentTariffForUpdate.HttpUnitTestsMaxNoBanner.Set(limits.HttpChecksMaxNoBanner);
            currentTariffForUpdate.UnitTestsMax.Set(limits.UnitTestsMax);
            currentTariffForUpdate.UnitTestsRequestsPerDay.Set(limits.UnitTestsRequestsPerDay);
            currentTariffForUpdate.UnitTestsMaxDays.Set(limits.UnitTestsMaxDays);
            currentTariffForUpdate.MetricsMax.Set(limits.MetricsMax);
            currentTariffForUpdate.MetricsRequestsPerDay.Set(limits.MetricRequestsPerDay);
            currentTariffForUpdate.MetricsMaxDays.Set(limits.MetricsMaxDays);
            currentTariffForUpdate.StorageSizeMax.Set(limits.StorageSizeMax);
            currentTariffForUpdate.SmsPerDay.Set(limits.SmsPerDay);

            accountStorage.TariffLimits.Update(currentTariffForUpdate);
        }
    }
}
