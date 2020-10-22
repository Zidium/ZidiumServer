using System;
using System.Web.Mvc;
using Zidium.Core.Common.Helpers;
using Zidium.Core.ConfigDb;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class AccountInfoController : BaseController
    {
        public ActionResult Index()
        {
            var account = DependencyInjection.GetServicePersistent<IConfigDbServicesFactory>().GetAccountService().GetSystemAccount();

            var model = new ShowAccountModel()
            {
                Id = account.Id,
                AccountName = account.SystemName,
                SecretKey = account.SecretKey,
                TariffName = AccountTypeHelper.GetDisplayName(account.Type),
                AccountType = account.Type
            };

            return View(model);
        }

        /// <summary>
        /// Для unit-тестов
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="userId"></param>
        public AccountInfoController(Guid accountId, Guid userId) : base(accountId, userId) { }

        public AccountInfoController() { }
    }
}