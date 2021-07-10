using Zidium.Api.Dto;

namespace Zidium.Api
{
    public interface IUnitTestTypeControl : IObjectControl
    {
        UnitTestTypeDto Info { get; }
    }
}
