using System;
using System.Collections.Generic;

namespace Zidium.Core.Api
{
    public class UpdateComponentRequestData
    {
        public Guid? Id { get; set; }

        public Guid? ParentId { get; set; }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }

        public Guid? TypeId { get; set; }

        public string Version { get; set; }

        public List<ExtentionPropertyDto> Properties { get; set; }
    }
}
