using System;

namespace Zidium.Api.XmlConfig
{
    public class DefaultEventValuesElement
    {
        public EventCategoryDefaultValuesElement ApplicationError { get; set; }

        public EventCategoryDefaultValuesElement ComponentEvent { get; set; }

        public DefaultEventValuesElement()
        {
            ApplicationError = new EventCategoryDefaultValuesElement()
            {
                JoinInterval = TimeSpan.FromMinutes(5)
            };

            ComponentEvent = new EventCategoryDefaultValuesElement()
            {
                JoinInterval = TimeSpan.FromMinutes(5)
            };
        }
    }
}
