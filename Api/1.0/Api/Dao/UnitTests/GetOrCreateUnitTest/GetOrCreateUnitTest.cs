using System;

namespace Zidium.Api
{
    public class GetOrCreateUnitTestData
    {
        public string SystemName { get; protected set; }

        public string DisplayName { get; set; }

        public IUnitTestTypeControl UnitTestTypeControl { get; set; }

        public GetOrCreateUnitTestData(string systemName)
        {
            if (systemName == null)
            {
                throw new ArgumentNullException("systemName");
            }
            SystemName = systemName;
        }
    }
}
