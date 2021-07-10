using System;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    public abstract class ComponentTypeControlBase : IComponentTypeControl
    {
        internal Client ClientInternal { get; set; }

        protected ComponentTypeControlBase(Client client)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }
            ClientInternal = client;
        }

        public abstract ComponentTypeDto Info { get; }

        public IClient Client
        {
            get { return ClientInternal; }
        }

        public string SystemName { get; protected set; }

        public abstract bool IsFake();
    }
}
