using System.Linq;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;

namespace Zidium.UserAccount.Models
{
    public class EventTypesListModel
    {
        public EventImportance? Importance { get; set; }

        public EventCategory? Category { get; set; }

        public string Search { get; set; }

        public bool ShowDeleted { get; set; }

        public IQueryable<EventType> EventTypes { get; set; }

        public static int MaxMessageLength = 150;
    }
}