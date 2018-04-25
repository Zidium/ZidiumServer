using System;

namespace Zidium.UserAccount.Models.ComponentHistory
{
    public class IndexModel
    {
        public TimelineInterval Interval { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public const string ExpandedItemsCookieName = "ComponentHistoryExpandedItems";
    }
}