using Zidium.Storage;

namespace Zidium.UserAccount.Models
{
    public class EventTypesListModel
    {
        public EventImportance? Importance { get; set; }

        public EventCategory? Category { get; set; }

        public string Search { get; set; }

        public EventTypeForRead[] EventTypes { get; set; }

        public static int MaxMessageLength = 150;
    }
}