using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    public class NotificationNaming : INaming
    {
        public string Nominative()
        {
            return "Уведомление";
        }

        public string NotFound()
        {
            return "не найдено";
        }

        public string Dative()
        {
            return "уведомлению";
        }
    }
}
