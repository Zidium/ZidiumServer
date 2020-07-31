using Zidium.Storage;

namespace Zidium.UserAccount.Models
{
    public class ComponentTypesListModel
    {
        public ComponentTypeForRead[] ComponentTypes { get; set; }

        public string Search { get; set; }
    }
}