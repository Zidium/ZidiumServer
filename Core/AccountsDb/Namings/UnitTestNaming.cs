using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    public class UnitTestNaming : INaming
    {
        public string Nominative()
        {
            return "Проверка";
        }

        public string NotFound()
        {
            return "не найдена";
        }

        public string Dative()
        {
            return "проверке";
        }
    }
}
