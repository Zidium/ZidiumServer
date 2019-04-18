using System;

namespace Zidium.Api
{
    public class SendUnitTestResultsData : SendUnitTestResultData
    {
        public Guid UnitTestId { get; set; }
    }
}
