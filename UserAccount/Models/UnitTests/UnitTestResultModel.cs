using System;
using System.Linq;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.UserAccount.Models.UnitTests;

namespace Zidium.UserAccount.Models
{
    public class UnitTestResultModel
    {
        public bool HasBanner { get; set; }

        public UnitTest UnitTest { get; set; }

        public Event[] Statuses { get; set; }

        public static readonly int LastStatusesMaxCount = 20;

        public OverviewCurrentStatusModel OverviewCurrentStatus { get; set; }
        public OverviewLastResultModel OverviewLastResult { get; set; }
        public OverviewSettingsHttpModel OverviewSettingsHttp { get; set; }

        public void Init(Guid unitTestId, AccountDbContext accountDbContext)
        {
            var accountId = FullRequestContext.Current.CurrentUser.AccountId;
            var unitTestRepository = accountDbContext.GetUnitTestRepository();
            UnitTest = unitTestRepository.GetById(unitTestId);
            HasBanner = UnitTest.HttpRequestUnitTest != null && UnitTest.HttpRequestUnitTest.HasBanner;

            var eventRepository = accountDbContext.GetEventRepository();
            var events = eventRepository.QueryAll(unitTestId, EventCategory.UnitTestStatus, null, null, null, null, null, LastStatusesMaxCount);

            Statuses = events.ToArray();
        }

        public string PageTitle
        {
            get
            {
                UnitTestType type = UnitTest.Type;
                if (type.IsSystem == false)
                {
                    return "Пользовательская проверка: " + UnitTest.DisplayName;
                }
                if (type.Id == SystemUnitTestTypes.HttpUnitTestType.Id)
                {
                    return "Проверка HTTP: " + UnitTest.DisplayName;
                }
                if (type.Id == SystemUnitTestTypes.SqlTestType.Id)
                {
                    return "Проверка SQL: " + UnitTest.DisplayName;
                }
                if (type.Id == SystemUnitTestTypes.PingTestType.Id)
                {
                    return "Проверка ping: " + UnitTest.DisplayName;
                }
                if (type.Id == SystemUnitTestTypes.SslTestType.Id)
                {
                    return "Проверка SSL сертификата: " + UnitTest.DisplayName;
                }
                if (type.Id == SystemUnitTestTypes.DomainNameTestType.Id)
                {
                    return "Проверка домена: " + UnitTest.DisplayName;
                }
                return "Проверка: " + UnitTest.DisplayName;
            }
        }
    }
}