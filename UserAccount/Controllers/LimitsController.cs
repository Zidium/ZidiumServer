using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Zidium.Core;
using Zidium.UserAccount.Models.Limits;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class LimitsController : ContextController
    {
        public ActionResult Index()
        {
            var client = GetDispatcherClient();
            var limitsResponse = client.GetAccountLimits(CurrentUser.AccountId, 30);

            if (!limitsResponse.Success)
                throw new UserFriendlyException("Не удалось получить лимиты аккаунта: " + limitsResponse.ErrorMessage);

            var unitTestRepository = CurrentAccountDbContext.GetUnitTestRepository();
            var unitTests = unitTestRepository.QueryAllWithDeleted().Include("Component").Include("Type");

            var model = new IndexModel()
            {
                Limits = limitsResponse.Data,
                UnitTests = unitTests.ToDictionary(t => t.Id, t => t)
            };

            return View(model);
        }

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="userId"></param>
        public LimitsController(Guid accountId, Guid userId) : base(accountId, userId) { }

        public LimitsController() { }

    }
}