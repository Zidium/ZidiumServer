using System;

namespace Zidium.UserAccount.Models
{
    public class StartsReportItemModel
    {
        public Guid ComponentId { get; set; }

        public string ComponentDisplayName {get; set; }

        public string ComponentSystemName { get; set; }

        public DateTime CreateDate { get; set; }

        public string Version { get; set; }

        public DateTime? FirstStart { get; set; }

        public Guid? FirstStartId { get; set; }

        public DateTime? LastStart { get; set; }

        public Guid? LastStartId { get; set; }

        public int Count { get; set; }
    }
}