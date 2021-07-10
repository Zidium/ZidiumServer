using System;

namespace Zidium.Api
{
    public class GetOrCreateComponentData
    {
        public GetOrCreateComponentData(string systemName, IComponentTypeControl componentTypeControl)
        {
            if (systemName == null)
            {
                throw new ArgumentNullException("systemName");
            }
            if (componentTypeControl == null)
            {
                throw new ArgumentNullException("componentTypeControl");
            }
            SystemName = systemName;
            ComponentTypeControl = componentTypeControl;
            Properties = new ExtentionPropertyCollection();
        }

        public IComponentTypeControl ComponentTypeControl { get; protected set; }

        public string SystemName { get; protected set; }

        public string DisplayName { get; set; }

        public string Version { get; set; }

        public ExtentionPropertyCollection Properties { get; protected set; }
    }
}
