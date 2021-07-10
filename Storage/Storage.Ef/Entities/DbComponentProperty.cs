using System;
using Zidium.Api.Dto;

namespace Zidium.Storage.Ef
{
    public class DbComponentProperty
    {
        public Guid Id { get; set; }

        public virtual DbComponent Component { get; set; }

        public Guid ComponentId { get; set; } 

        public string Name { get; set; }

        public string Value { get; set; }

        public DataType DataType { get; set; }
    }
}
