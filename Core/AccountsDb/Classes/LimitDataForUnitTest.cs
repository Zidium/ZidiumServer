using System;

namespace Zidium.Core.AccountsDb
{
    public class LimitDataForUnitTest
    {
        public Guid Id { get; set; }

        public Guid LimitDataId { get; set; }

        public virtual LimitData LimitData { get; set; }

        public Guid UnitTestId { get; set; }

        public virtual UnitTest UnitTest { get; set; }

        public int ResultsCount { get; set; }
    }
}
