using System;
using Zidium.Core.Api;

namespace Zidium.Core.AccountsDb
{
    public class LogProperty
    {
        public Guid Id { get; set; }

        public Guid LogId { get; set; }

        public virtual Log Log { get; set; }

        public string Name { get; set; }

        public DataType DataType { get; set; }

        public string Value { get; set; }
    }
}
