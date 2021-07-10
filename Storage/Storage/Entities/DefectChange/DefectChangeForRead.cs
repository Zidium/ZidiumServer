using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Информация об изменении дефекта (история)
    /// </summary>
    public class DefectChangeForRead
    {
        public DefectChangeForRead(
            Guid id, 
            Guid defectId, 
            DateTime date, 
            DefectStatus status, 
            string comment, 
            Guid? userId)
        {
            Id = id;
            DefectId = defectId;
            Date = date;
            Status = status;
            Comment = comment;
            UserId = userId;
        }

        public Guid Id { get; }

        public Guid DefectId { get; }

        public DateTime Date { get; }

        public DefectStatus Status { get; }

        public string Comment { get; }

        public Guid? UserId { get; }

    }
}
