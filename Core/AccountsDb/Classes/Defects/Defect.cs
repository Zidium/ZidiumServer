using System;
using System.Collections.Generic;

namespace Zidium.Core.AccountsDb.Classes
{
    public class Defect
    {
        public Defect()
        {
            Changes = new HashSet<DefectChange>();
        }

        public Guid Id { get; set; }

        public int Number { get; set; }

        public string Title { get; set; }

        public Guid? LastChangeId { get; set; }

        public virtual DefectChange LastChange { get; set; }

        public Guid? ResponsibleUserId { get; set; }

        public virtual User ResponsibleUser { get; set; }

        public Guid? EventTypeId { get; set; }

        public virtual EventType EventType { get; set; }

        public string Notes { get; set; }

        public virtual ICollection<DefectChange> Changes { get; set; }

        public string GetCode()
        {
            return Number.ToString();
        }

        public DefectStatus GetStatus()
        {
            if (LastChange == null)
            {
                return DefectStatus.Unknown;
            }
            return LastChange.Status;
        }
    }
}
