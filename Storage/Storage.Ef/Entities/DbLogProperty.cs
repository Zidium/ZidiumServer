using System;

namespace Zidium.Storage.Ef
{
    public class DbLogProperty
    {
        public Guid Id { get; set; }

        public Guid LogId { get; set; }

        public virtual DbLog Log { get; set; }

        public string Name { get; set; }

        public DataType DataType { get; set; }

        public string Value { get; set; }
    }
}
