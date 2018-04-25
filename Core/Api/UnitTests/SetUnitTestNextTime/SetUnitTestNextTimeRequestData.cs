using System;

namespace Zidium.Core.Api
{
    public class SetUnitTestNextTimeRequestData
    {
        public Guid? UnitTestId { get; set; }

        public DateTime? NextTime { get; set; }
    }
}
