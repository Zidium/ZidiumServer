using System;
using System.Collections.Generic;

namespace Zidium.UserAccount.Models
{
    public class BugTrackerModel
    {
        public Guid? ComponentId { get; set; }
        public List<BugTrackerEventModel> Events { get; set; }
    }
}