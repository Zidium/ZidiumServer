using System;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.Defects
{
    public class ShowDefectModel
    {
        public EventTypeForRead EventType { get; set; }

        public DefectForRead Defect { get; set; }

        public DefectChangeInfo[] Changes { get; set; }

        public string LastChangeUser { get; set; }

        public string ResponsibleUser { get; set; }

        public class DefectChangeInfo
        {
            public DateTime Date;

            public DefectStatus Status;

            public string Comment;

            public string UserLogin;
        }
    }
}