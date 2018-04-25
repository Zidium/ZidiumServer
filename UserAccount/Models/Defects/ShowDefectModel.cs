using Zidium.Core.AccountsDb;
using Zidium.Core.AccountsDb.Classes;

namespace Zidium.UserAccount.Models.Defects
{
    public class ShowDefectModel
    {
        public EventType EventType { get; set; }

        public Defect Defect { get; set; }
    }
}