using System;

namespace Zidium.Core.Api
{
    public class GetOrCreateUnitTestResponseData
    {
        public Guid Id { get; set; }

        public Guid TypeId { get; set; }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }
    }
}
