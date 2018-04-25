using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    public class ComponentPropertyNaming : INaming
    {
        public string Nominative()
        {
            return "Свойство компонента";
        }

        public string NotFound()
        {
            return "не найдено";
        }

        public string Dative()
        {
            return "свойству компонента";
        }
    }
}
