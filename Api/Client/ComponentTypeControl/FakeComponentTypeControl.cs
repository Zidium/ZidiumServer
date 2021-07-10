using Zidium.Api.Dto;

namespace Zidium.Api
{
    public class FakeComponentTypeControl : IComponentTypeControl
    {
        public void Dispose()
        {
        }

        public IClient Client { get { return null; } }

        public string SystemName { get { return null; } }

        public bool IsFake()
        {
            return true;
        }

        public void Detach()
        {
        }

        public ComponentTypeDto Info { get { return null; } }
    }
}
