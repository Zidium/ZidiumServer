using System;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.Defects
{
    public class DefectLastErrorModel
    {
        public EventForRead Event { get; set; }

        public Guid ComponentId { get; set; }

        public string ComponentFullName { get; set; }
    }
}