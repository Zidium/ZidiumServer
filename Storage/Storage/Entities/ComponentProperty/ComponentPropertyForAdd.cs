using System;
using Zidium.Api.Dto;

namespace Zidium.Storage
{
    public class ComponentPropertyForAdd
    {
        public Guid Id;

        public Guid ComponentId;

        public string Name;

        public string Value;

        public DataType DataType;
    }
}
