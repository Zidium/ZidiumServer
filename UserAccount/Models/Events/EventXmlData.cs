using System;
using System.Xml.Serialization;

namespace Zidium.UserAccount.Models.Events
{
    [XmlType("Event")]
    public class EventXmlData
    {
        public Guid Id { get; set; }

        public Guid EventTypeId { get; set; }

        public string EventTypeSystemName { get; set; }

        public string EventTypeDisplayName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime ActualDate { get; set; }

        public int Count { get; set; }

        public string Message { get; set; }

        public string TypeCode { get; set; }

        public long JoinKeyHash { get; set; }

        public string Category { get; set; }

        public Guid OwnerId { get; set; }

        public Guid ComponentId { get; set; }

        public string ComponentSystemName { get; set; }

        public string ComponentDisplayName { get; set; }

        [XmlArrayItem("EventProperty")]
        public EventPropertyXml[] Properties { get; set; }

    }
}