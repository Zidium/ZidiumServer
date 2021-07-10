using System;

namespace Zidium.Api.Dto
{
    public class SetUnitTestDisableRequestDataDto
    {
        public Guid? UnitTestId { get; set; }

        public DateTime? ToDate { get; set; }

        public string Comment { get; set; }
    }
}
