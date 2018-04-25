using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    public class UserContactNaming : INaming
    {
        public string Nominative()
        {
            return "Контакт";
        }

        public string NotFound()
        {
            return "не найден";
        }

        public string Dative()
        {
            return "контакту";
        }
    }
}
