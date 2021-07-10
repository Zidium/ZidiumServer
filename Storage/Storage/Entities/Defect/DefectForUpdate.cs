using System;

namespace Zidium.Storage
{
    public class DefectForUpdate
    {
        public DefectForUpdate(Guid id)
        {
            Id = id;
            Number = new ChangeTracker<int>();
            Title = new ChangeTracker<string>();
            LastChangeId = new ChangeTracker<Guid?>();
            ResponsibleUserId = new ChangeTracker<Guid?>();
            EventTypeId = new ChangeTracker<Guid?>();
            Notes = new ChangeTracker<string>();
        }

        public Guid Id { get; }

        public ChangeTracker<int> Number { get; }

        public ChangeTracker<string> Title { get; }

        public ChangeTracker<Guid?> LastChangeId { get; }

        public ChangeTracker<Guid?> ResponsibleUserId { get; }

        public ChangeTracker<Guid?> EventTypeId { get; }

        public ChangeTracker<string> Notes { get; }

    }
}