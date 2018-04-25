using System;
using Zidium.Core.Api;

namespace Zidium.Core.AccountsDb
{
    public class ComponentProperty
    {
        public Guid Id { get; set; }

        public virtual Component Component { get; set; }

        public Guid ComponentId { get; set; } 

        public string Name { get; set; }

        public string Value { get; set; }

        public DataType DataType { get; set; }
    }
}
