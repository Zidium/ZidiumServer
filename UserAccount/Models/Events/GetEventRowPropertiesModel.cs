using Zidium.Storage;

namespace Zidium.UserAccount.Models.Events
{
    public class GetEventRowPropertiesModel
    {
        public EventForRead Event;

        public EventPropertyForRead[] Properties;
    }
}