using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Models
{
    public class DashboardEventsByTypeModel
    {
        public EventType EventType {get;set;}

        public int Count { get; set; }
    }
}