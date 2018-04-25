using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    public class ProblemNaming : INaming
    {
        public string Nominative()
        {
            return "Проблема";
        }

        public string NotFound()
        {
            return "не найдена";
        }

        public string Dative()
        {
            return "проблеме";
        }
    }
}
