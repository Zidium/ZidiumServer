using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    public class ComponentTypeNaming : INaming
    {
        public string Nominative()
        {
            return "Тип компонента";
        }

        public string NotFound()
        {
            return "не найден";
        }

        public string Dative()
        {
            return "типу компонента";
        }
    }
}
