using System;

namespace Zidium.Api
{
    public class ComponentTypeControlOnline : ComponentTypeControlBase
    {
        public ComponentTypeControlOnline(Client client, ComponentTypeInfo info)
            : base(client)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            InfoInternal = info;
            SystemName = info.SystemName;
        }

        protected ComponentTypeInfo InfoInternal { get; set; }

        public override ComponentTypeInfo Info
        {
            get { return InfoInternal; }
        }

        public override bool IsFake()
        {
            return false;
        }
    }
}
