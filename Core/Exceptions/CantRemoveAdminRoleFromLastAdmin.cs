using Zidium.Core.Common;

namespace Zidium.Core
{
    public class CantRemoveAdminRoleFromLastAdmin : UserFriendlyException
    {
        public CantRemoveAdminRoleFromLastAdmin() : base("Нельзя удалить права у единственного адинистратора") { }
    }
}
