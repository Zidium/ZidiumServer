using System;
using Zidium.Core.Api;

namespace Zidium.Core.AccountsDb.Classes.UnitTests
{
    public class UnitTestProperty
    {
        public Guid Id { get; set; }

        public virtual UnitTest UnitTest { get; set; }

        public Guid UnitTestId { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public DataType DataType { get; set; }
    }
}
