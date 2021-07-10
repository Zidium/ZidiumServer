using System;

namespace Zidium.Storage.Ef
{
    public class DbLimitDataForUnitTest
    {
        public Guid Id { get; set; }

        public Guid LimitDataId { get; set; }

        public virtual DbLimitData LimitData { get; set; }

        public Guid UnitTestId { get; set; }

        public virtual DbUnitTest UnitTest { get; set; }

        public int ResultsCount { get; set; }
    }
}
