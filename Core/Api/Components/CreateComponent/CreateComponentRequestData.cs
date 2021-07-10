using System;
using System.Collections.Generic;
using Zidium.Api.Dto;

namespace Zidium.Core.Api
{
    public class CreateComponentRequestData
    {
        public Guid? ParentComponentId { get; set; }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }

        public Guid? TypeId { get; set; }

        public string Version { get; set; }

        public List<ExtentionPropertyDto> Properties { get; set; }
    }
}
