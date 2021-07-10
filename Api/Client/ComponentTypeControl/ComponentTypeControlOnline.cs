using System;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    public class ComponentTypeControlOnline : ComponentTypeControlBase
    {
        public ComponentTypeControlOnline(Client client, ComponentTypeDto info)
            : base(client)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            InfoInternal = info;
            SystemName = info.SystemName;
        }

        protected ComponentTypeDto InfoInternal { get; set; }

        public override ComponentTypeDto Info
        {
            get { return InfoInternal; }
        }

        public override bool IsFake()
        {
            return false;
        }
    }
}
