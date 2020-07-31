using System;

namespace Zidium.Storage
{
    public class DefectChangeForAdd
    {
        public Guid Id;

        public Guid DefectId;

        public DateTime Date;

        public DefectStatus Status;

        public string Comment;

        public Guid? UserId;
    }
}
