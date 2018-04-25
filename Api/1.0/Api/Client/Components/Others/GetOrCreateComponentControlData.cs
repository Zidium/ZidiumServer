using System;

namespace Zidium.Api
{
    internal class GetOrCreateComponentControlData
    {
        public ComponentControlWrapper Parent { get; protected set; }

        public GetOrCreateComponentData Data { get; protected set; }

        public GetOrCreateComponentControlData(ComponentControlWrapper parent, GetOrCreateComponentData data)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            Parent = parent;
            Data = data;
        }
    }
}
