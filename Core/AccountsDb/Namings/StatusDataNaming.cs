using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    public class StatusDataNaming : INaming
    {
        public string Nominative()
        {
            return "Данные статуса";
        }

        public string NotFound()
        {
            return "не найдены";
        }

        public string Dative()
        {
            return "данным статуса";
        }
    }
}
