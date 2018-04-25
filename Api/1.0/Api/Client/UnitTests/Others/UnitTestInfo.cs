using System;
using Zidium.Api.Dto;

namespace Zidium.Api
{
    public class UnitTestInfo
    {
        public Guid Id { get; protected set; }

        public Guid TypeId { get; protected set; }

        public string SystemName { get; protected set; }

        public string DisplayName { get; protected set; }

        public UnitTestInfo(UnitTestDto data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            Id = data.Id;
            TypeId = data.TypeId;
            SystemName = data.SystemName;
            DisplayName = data.DisplayName;
        }
    }
}
