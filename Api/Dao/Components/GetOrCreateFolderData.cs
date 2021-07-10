using System;

namespace Zidium.Api
{
    public class GetOrCreateFolderData
    {
        public GetOrCreateFolderData(string systemName)
        {
            if (systemName == null)
            {
                throw new ArgumentNullException("systemName");
            }
            SystemName = systemName;
            Properties = new ExtentionPropertyCollection();
        }

        public string SystemName { get; protected set; }

        public string DisplayName { get; set; }

        public ExtentionPropertyCollection Properties { get; protected set; }
    }
}
