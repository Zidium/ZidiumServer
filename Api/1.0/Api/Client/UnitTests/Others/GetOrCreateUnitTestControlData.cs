using System;

namespace Zidium.Api
{
    internal class GetOrCreateUnitTestControlData
    {
        internal ComponentControlWrapper Component { get; set; }

        public GetOrCreateUnitTestData Data { get; protected set; }

        internal GetOrCreateUnitTestControlData(
            ComponentControlWrapper component,
            GetOrCreateUnitTestData data)
        {
            if (component == null)
            {
                throw new ArgumentNullException("component");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            Component = component;
            Data = data;
        }
    }
}
