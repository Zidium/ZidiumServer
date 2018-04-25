using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    public class CounterNaming : INaming
    {
        public string Nominative()
        {
            return "Метрика";
        }

        public string NotFound()
        {
            return "не найдена";
        }

        public string Dative()
        {
            return "метрике";
        }
    }
}
