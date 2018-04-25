using Zidium.Api;

namespace Zidium.Core.Common.Helpers
{
    public class HttpServiceEventPreparer : IEventPreparer
    {
        public void Prepare(SendEventBase eventObj)
        {
            HttpContextHelper.SetProperties(eventObj.Properties);
        }
    }
}
