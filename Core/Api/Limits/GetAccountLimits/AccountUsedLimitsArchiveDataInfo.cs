using System;

namespace Zidium.Core.Api
{
    public class AccountUsedLimitsArchiveDataInfo
    {
        public DateTime Date { get; set; }

        public AccountUsedLimitsPerDayDataInfo Info { get; set; }
    }
}
