using System;

namespace Zidium.UserAccount.Models
{
    public class ComponentShowTimelinesPartialModel
    {
        public Guid ComponentId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}