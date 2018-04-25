using Zidium.Core.Common;

namespace Zidium.Core
{
    public class AccountIsBlockedException : UserFriendlyException
    {
        public AccountIsBlockedException() : base ("Извините, ваш аккаунт заблокирован") {}
    }
}
