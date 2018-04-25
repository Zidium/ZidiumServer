using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Models.Defects
{
    public class DefectLastErrorModel
    {
        public Event Event { get; set; }

        public Component Component { get; set; }
    }
}