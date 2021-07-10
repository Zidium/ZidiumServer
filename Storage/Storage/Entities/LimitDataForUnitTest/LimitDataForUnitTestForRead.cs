using System;

namespace Zidium.Storage
{
    public class LimitDataForUnitTestForRead
    {
        public LimitDataForUnitTestForRead(Guid id, Guid limitDataId, Guid unitTestId, int resultsCount)
        {
            Id = id;
            LimitDataId = limitDataId;
            UnitTestId = unitTestId;
            ResultsCount = resultsCount;
        }

        public Guid Id { get; }

        public Guid LimitDataId { get; }

        public Guid UnitTestId { get; }

        public int ResultsCount { get; }
    }
}
