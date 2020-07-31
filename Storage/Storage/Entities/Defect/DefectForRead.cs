using System;

namespace Zidium.Storage
{
    public class DefectForRead
    {
        public DefectForRead(
            Guid id, 
            int number, 
            string title, 
            Guid? lastChangeId, 
            Guid? responsibleUserId, 
            Guid? eventTypeId, 
            string notes)
        {
            Id = id;
            Number = number;
            Title = title;
            LastChangeId = lastChangeId;
            ResponsibleUserId = responsibleUserId;
            EventTypeId = eventTypeId;
            Notes = notes;
        }

        public Guid Id { get; }

        public int Number { get; }

        public string Title { get; }

        public Guid? LastChangeId { get; }

        public Guid? ResponsibleUserId { get; }

        public Guid? EventTypeId { get; }

        public string Notes { get; }

        public DefectForUpdate GetForUpdate()
        {
            return new DefectForUpdate(Id);
        }

    }
}
