using Zidium.Core.Common;

namespace Zidium.Core
{
    public class CantDeleteLastUserException : UserFriendlyException
    {
        public CantDeleteLastUserException() : base("Нельзя удалить последнего пользователя аккаунта") { }
    }
}
