using System;

namespace Zidium.Api
{
    public class GetOrCreateUnitTestTypeData
    {
        public string SystemName { get; protected set; }

        public string DisplayName { get; set; }

        public GetOrCreateUnitTestTypeData(string systemName)
        {
            if (systemName == null)
            {
                throw new ArgumentNullException("systemName");
            }
            SystemName = systemName;
        }
    }
}
