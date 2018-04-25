using System;

namespace Zidium.Core.Api
{
    public class GetAllAccountsLimitsResponseData
    {
        public Guid AccountId { get; set; }

        public GetAccountLimitsResponseData Limits { get; set; }
    }
}
