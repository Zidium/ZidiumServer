using System;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    public class UnitTestTypeControlOnline : UnitTestTypeControlBase
    {
        public UnitTestTypeControlOnline(
            Client client,
            string systemName,
            UnitTestTypeDto info)
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

        protected UnitTestTypeDto InfoInternal { get; set; }

        public override UnitTestTypeDto Info
        {
            get { return InfoInternal; }
        }
    }
}
