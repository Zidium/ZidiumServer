using System;

namespace Zidium.Api
{
    /// <summary>
    /// Если свойство null - значит его обновлять НЕ нужно
    /// </summary>
    public class UpdateComponentData
    {
        public UpdateComponentData()
        {
            Properties = new ExtentionPropertyCollection();    
        }

        public Guid? ParentId { get; set; }

        public string SystemName { get; set; }

        public string DisplayName { get; set; }

        public Guid? TypeId { get; set; }

        public string Version { get; set; }

        public ExtentionPropertyCollection Properties { get; set; }
    }
}
