using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    public class UnitTestTypeNaming : INaming
    {
        public string Nominative()
        {
            return "Тип юнит-теста";
        }

        public string NotFound()
        {
            return "не найден";
        }

        public string Dative()
        {
            return "типу юнит-теста";
        }
    }
}
