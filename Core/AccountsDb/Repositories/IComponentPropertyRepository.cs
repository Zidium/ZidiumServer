using Zidium.Core.Api;

namespace Zidium.Core.AccountsDb
{
    public interface IComponentPropertyRepository : IAccountBasedRepository<ComponentProperty>
    {
        ComponentProperty Update(ComponentProperty property, string name, string value, DataType datatype);

        ComponentProperty Add(Component component, string name, string value, DataType datatype);
    }
}
