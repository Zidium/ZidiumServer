using System;
using System.Collections.Generic;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;

namespace Zidium.UserAccount.Models.Limits
{
    public class IndexModel
    {
        public GetAccountLimitsResponseData Limits { get; set; }

        public Dictionary<Guid, UnitTest> UnitTests { get; set; }
    }
}