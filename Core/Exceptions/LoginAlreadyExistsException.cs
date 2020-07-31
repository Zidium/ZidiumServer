using Zidium.Common;
using Zidium.Core.Common;

namespace Zidium.Core
{
    public class LoginAlreadyExistsException : UserFriendlyException
    {
        public LoginAlreadyExistsException(string login) : base("Логин " + login + " уже существует") { }
    }
}
