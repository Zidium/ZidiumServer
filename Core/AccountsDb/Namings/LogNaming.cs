using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    public class LogNaming : INaming
    {
        public string Nominative()
        {
            return "Запись лога";
        }

        public string NotFound()
        {
            return "не найдена";
        }

        public string Dative()
        {
            return "записи лога";
        }
    }
}
