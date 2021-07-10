using System;
using System.Collections.Generic;

namespace Zidium.Storage.Ef
{
    public class DbDefect
    {
        public DbDefect()
        {
            Changes = new HashSet<DbDefectChange>();
        }

        public Guid Id { get; set; }

        public int Number { get; set; }

        public string Title { get; set; }

        public Guid? LastChangeId { get; set; }

        public virtual DbDefectChange LastChange { get; set; }

        public Guid? ResponsibleUserId { get; set; }

        public virtual DbUser ResponsibleUser { get; set; }

        public Guid? EventTypeId { get; set; }

        public virtual DbEventType EventType { get; set; }

        public string Notes { get; set; }

        public virtual ICollection<DbDefectChange> Changes { get; set; }

    }
}
