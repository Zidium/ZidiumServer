using System;
using Zidium.Api.Common;

namespace Zidium.Api
{
    public class UnitTestTypeControlOnline : UnitTestTypeControlBase
    {
        public UnitTestTypeControlOnline(
            Client client,
            string systemName,
            UnitTestTypeInfo info)
            : base(client, systemName)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            InfoInternal = info;
        }

        public override bool IsFake()
        {
            return false;
        }

        protected UnitTestTypeInfo InfoInternal { get; set; }

        public override UnitTestTypeInfo Info
        {
            get { return InfoInternal; }
        }
    }
}
