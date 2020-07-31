using System;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.Defects
{
    public class ChangeDefectStatusDialogModel
    {
        public string DefectCode { get; set; }

        public Guid? DefectId { get; set; }

        public DefectStatus? Status { get; set; }

        public string Comment { get; set; }
    }
}   