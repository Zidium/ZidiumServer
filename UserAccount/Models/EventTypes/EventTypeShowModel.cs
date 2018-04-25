using System;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;

namespace Zidium.UserAccount.Models
{
    public class EventTypeShowModel
    {
        public Guid Id { get; set; }

        public string DisplayName { get; set; }

        public string SystemName { get; set; }

        public ComponentType ComponentType { get; set; }

        public EventCategory Category { get; set; }

        public string Code { get; set; }

        public TimeSpan? JoinInterval { get; set; }

        public string OldVersion { get; set; }

        public EventImportance? ImportanceForOld { get; set; }

        public EventImportance? ImportanceForNew { get; set; }

        public bool IsSystem { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? Created { get; set; }

        public TimelineInterval Period { get; set; }
    }
}