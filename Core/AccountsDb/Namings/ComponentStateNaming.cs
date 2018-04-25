using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    public class ComponentStateNaming : INaming
    {
        public string Nominative()
        {
            return "Состояние компонента";
        }

        public string NotFound()
        {
            return "не найдено";
        }

        public string Dative()
        {
            return "состоянию компонента";
        }
    }
}
