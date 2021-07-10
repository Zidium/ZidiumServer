using System;
using System.Collections.Generic;

namespace Zidium.Api.Dto
{
    public class ComponentDto
    {
        public Guid Id { get; set; }

        public Guid? ParentId { get; set; }
        
        public ComponentTypeDto Type { get; set; }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }

        public DateTime CreatedDate { get; set; }

        public string Version { get; set; }

        public bool IsRoot()
        {
             return ParentId == null; 
        }

        public List<ExtentionPropertyDto> Properties { get; set; }
    }
}
