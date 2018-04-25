using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    public class EventTypeNaming : INaming
    {
        public string Nominative()
        {
            return "Тип события";
        }

        public string NotFound()
        {
            return "не найден";
        }

        public string Dative()
        {
            return "типу события";
        }
    }
}
