﻿using System;
using System.Linq;
using System.Web.Mvc;
using Zidium.Common;
using Zidium.Core;
using Zidium.UserAccount.Models.Limits;

namespace Zidium.UserAccount.Controllers
{
    [Authorize]
    public class LimitsController : BaseController
    {
        public ActionResult Index()
        {
            var client = GetDispatcherClient();
            var limitsResponse = client.GetAccountLimits(CurrentUser.AccountId, 30);

            if (!limitsResponse.Success)
                throw new UserFriendlyException("Не удалось получить лимиты аккаунта: " + limitsResponse.ErrorMessage);

            var unitTests = GetStorage().UnitTests.GetAllWithDeleted();
            var unitTestTypes = GetStorage().UnitTestTypes.GetAllWithDeleted().ToDictionary(a => a.Id, b => b);
            var components = GetStorage().Components.GetMany(unitTests.Select(t => t.ComponentId).Distinct().ToArray()).ToDictionary(a => a.Id, b => b);

            var model = new IndexModel()
            {
                Limits = limitsResponse.Data,
                UnitTests = unitTests.ToDictionary(t => t.Id, t => new IndexModel.UnitTestInfo()
                {
                    Id = t.Id,
                    Type = new IndexModel.UnitTestTypeInfo()
                    {
                        Id = t.TypeId,
                        DisplayName = unitTestTypes[t.TypeId].DisplayName
                    },
                    DisplayName = t.DisplayName,
                    Component = new IndexModel.ComponentInfo()
                    {
                        DisplayName = components[t.ComponentId].DisplayName
                    }
                })
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