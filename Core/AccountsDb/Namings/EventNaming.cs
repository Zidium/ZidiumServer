using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    public class EventNaming : INaming
    {
        public string Nominative()
        {
            return "Событие";
        }

        public string NotFound()
        {
            return "не найдено";
        }

        public string Dative()
        {
            return "событию";
        }
    }
}
