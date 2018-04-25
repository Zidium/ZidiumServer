using System;
using System.Linq;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;

namespace Zidium.UserAccount.Models
{
    public class UnitTestResultModel
    {
        public bool HasBanner { get; set; }

        public UnitTest UnitTest { get; set; }

        public Event[] Statuses { get; set; }

        public static readonly int LastStatusesMaxCount = 20;

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
    }
}