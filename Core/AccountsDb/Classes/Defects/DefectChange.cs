using System;

namespace Zidium.Core.AccountsDb.Classes
{
    /// <summary>
    /// Информация об изменении дефекта (история)
    /// </summary>
    public class DefectChange
    {
        public Guid Id { get; set; }

        public Guid DefectId { get; set; }

        public virtual Defect Defect { get; set; }

        public DateTime Date { get; set; }

        public DefectStatus Status { get; set; }

        public string Comment { get; set; }

        public Guid? UserId { get; set; }

        public virtual User User { get; set; }
    }
}
