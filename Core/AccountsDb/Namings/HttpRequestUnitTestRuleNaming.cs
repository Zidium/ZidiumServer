using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    public class HttpRequestUnitTestRuleNaming : INaming
    {
        public string Nominative()
        {
            return "Правило юнит-теста";
        }

        public string NotFound()
        {
            return "не найдено";
        }

        public string Dative()
        {
            return "правилу юнит-теста";
        }
    }
}
