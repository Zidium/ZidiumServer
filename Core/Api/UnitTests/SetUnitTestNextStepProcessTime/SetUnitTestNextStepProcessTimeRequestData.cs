using System;

namespace Zidium.Core.Api
{
    public class SetUnitTestNextStepProcessTimeRequestData
    {
        public Guid? UnitTestId { get; set; }

        public DateTime? NextStepProcessTime { get; set; }
    }
}
