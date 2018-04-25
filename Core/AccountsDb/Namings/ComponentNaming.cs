using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    public class ComponentNaming : INaming
    {
        public string Nominative()
        {
            return "Компонент";
        }

        public string NotFound()
        {
            return "не найден";
        }

        public string Dative()
        {
            return "компоненту";
        }
    }
}
