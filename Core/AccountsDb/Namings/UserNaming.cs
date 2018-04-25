using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    public class UserNaming : INaming
    {
        public string Nominative()
        {
            return "Пользователь";
        }

        public string NotFound()
        {
            return "не найден";
        }

        public string Dative()
        {
            return "пользователю";
        }
    }
}
