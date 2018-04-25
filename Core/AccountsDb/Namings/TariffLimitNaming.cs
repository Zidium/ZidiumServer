using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    public class TariffLimitNaming : INaming
    {
        public string Nominative()
        {
            return "Тариф";
        }

        public string NotFound()
        {
            return "не найден";
        }

        public string Dative()
        {
            return "тарифу";
        }
    }
}
