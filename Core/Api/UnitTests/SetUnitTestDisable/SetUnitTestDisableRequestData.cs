using System;

namespace Zidium.Core.Api
{
    public class SetUnitTestDisableRequestData
    {
        public Guid? UnitTestId { get; set; }

        public DateTime? ToDate { get; set; }

        public string Comment { get; set; }
    }
}
