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

        public ComponentTypeInfo Info { get { return null; } }
    }
}
