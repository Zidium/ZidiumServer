using System;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.ComponentTreeDetails
{
    public class ComponentDetailsInfoModel
    {
        public Guid ComponentId { get; set; }
        public string SystemName { get; set; }
        public string TypeName { get; set; }
        public Guid TypeId { get; set; }
        public DateTime CreateTime { get; set; }
        public string Version { get; set; }
        public ComponentPropertyForRead[] Properties { get; set; }
    }
}