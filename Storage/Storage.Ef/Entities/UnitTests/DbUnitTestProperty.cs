using System;

namespace Zidium.Storage.Ef
{
    public class DbUnitTestProperty
    {
        public Guid Id { get; set; }

        public virtual DbUnitTest UnitTest { get; set; }

        public Guid UnitTestId { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public DataType DataType { get; set; }
    }
}
