using System;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    public abstract class UnitTestTypeControlBase : IUnitTestTypeControl
    {
        public abstract UnitTestTypeDto Info { get; }

        internal Client ClientInternal { get; set; }

        public string SystemName { get; protected set; }

        protected UnitTestTypeControlBase(Client client, string systemName)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }
            if (systemName == null)
            {
                throw new ArgumentNullException("systemName");
            }
            ClientInternal = client;
            SystemName = systemName;
        }

        public IClient Client
        {
            get { return ClientInternal; }
        }

        public abstract bool IsFake();
    }
}
