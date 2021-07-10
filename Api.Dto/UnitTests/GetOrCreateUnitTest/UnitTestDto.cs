using System;

namespace Zidium.Api.Dto
{
    public class UnitTestDto
    {
        public Guid Id { get; set; }

        public Guid TypeId { get; set; }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }
    }
}
