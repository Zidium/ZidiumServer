using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    public class SubscriptionNaming : INaming
    {
        public string Nominative()
        {
            return "Подписка";
        }

        public string NotFound()
        {
            return "не найдена";
        }

        public string Dative()
        {
            return "подписке";
        }
    }
}
