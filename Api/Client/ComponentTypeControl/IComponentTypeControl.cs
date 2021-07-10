using Zidium.Api.Dto;

namespace Zidium.Api
{
    public interface IComponentTypeControl : IObjectControl
    {
        ComponentTypeDto Info { get; }
    }
}
