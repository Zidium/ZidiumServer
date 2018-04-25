using Zidium.Core.Common;

namespace Zidium.Core
{
    public class CantDeleteLastAdminException : UserFriendlyException
    {
        public CantDeleteLastAdminException() : base("Нельзя удалить единственного администратора") { }
    }
}
