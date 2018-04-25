using System;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.DispatcherLayer;

namespace Zidium.Core.ConfigDb
{
    public class AccountManagementService : IAccountManagementService
    {
        public AccountManagementService(DispatcherContext context)
        {
            Context = context;
        }

        protected DispatcherContext Context;

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

        public void SetAccountLimits(Guid accountId, AccountTotalLimitsDataInfo limits, TariffLimitType type)
        {
            var accountContext = Context.GetAccountDbContext(accountId);
            var accountTariffRepository = accountContext.GetAccountTariffRepository();
            var currentTariff = accountTariffRepository.GetBaseTariffLimit(type);

            currentTariff.EventsRequestsPerDay = limits.EventRequestsPerDay;
            currentTariff.EventsMaxDays = limits.EventsMaxDays;
            currentTariff.LogMaxDays = limits.LogMaxDays;
            currentTariff.LogSizePerDay = limits.LogSizePerDay;
            currentTariff.ComponentsMax = limits.ComponentsMax;
            currentTariff.ComponentTypesMax = limits.ComponentTypesMax;
            currentTariff.UnitTestTypesMax = limits.UnitTestTypesMax;
            currentTariff.HttpUnitTestsMaxNoBanner = limits.HttpChecksMaxNoBanner;
            currentTariff.UnitTestsMax = limits.UnitTestsMax;
            currentTariff.UnitTestsRequestsPerDay = limits.UnitTestsRequestsPerDay;
            currentTariff.UnitTestsMaxDays = limits.UnitTestsMaxDays;
            currentTariff.MetricsMax = limits.MetricsMax;
            currentTariff.MetricsRequestsPerDay = limits.MetricRequestsPerDay;
            currentTariff.MetricsMaxDays = limits.MetricsMaxDays;
            currentTariff.StorageSizeMax = limits.StorageSizeMax;
            currentTariff.SmsPerDay = limits.SmsPerDay;
        }

        public void ValidateAccountName(string name)
        {
            throw new NotImplementedException();
        }
    }
}
