using System;

namespace Zidium.Api.Dto
{
    public class GetOrCreateUnitTestRequestDtoData
    {
        public Guid? ComponentId { get; set; }

        public Guid? UnitTestTypeId { get; set; }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }
    }
}
