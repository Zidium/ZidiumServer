using System;

namespace Zidium.Storage.Ef
{
    /// <summary>
    /// Информация об изменении дефекта (история)
    /// </summary>
    public class DbDefectChange
    {
        public Guid Id { get; set; }

        public Guid DefectId { get; set; }

        public virtual DbDefect Defect { get; set; }

        public DateTime Date { get; set; }

        public DefectStatus Status { get; set; }

        public string Comment { get; set; }

        public Guid? UserId { get; set; }

        public virtual DbUser User { get; set; }
    }
}
