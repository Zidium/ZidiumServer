using System.Linq;
using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Models
{
    public class ComponentTypesListModel
    {
        public IQueryable<ComponentType> ComponentTypes { get; set; }

        public string Search { get; set; }
    }
}