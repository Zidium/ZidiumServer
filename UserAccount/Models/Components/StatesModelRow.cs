using System;
using System.Linq;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Models.Components
{
    public class StatesModelRow
    {
        public StatesModelRow()
        {
            Events = new ColorCountGroup();
            Checks = new ColorCountGroup();
            Childs = new ColorCountGroup();
            Counters = new ColorCountGroup();
        }

        public Guid ComponentTypeId { get; set; }

        public string ComponentTypeName { get; set; }

        public Guid ComponentId { get; set; }

        public string ComponentName { get; set; }

        public ColorCountGroup Events { get; set; }

        public ColorCountGroup Checks { get; set; }

        public ColorCountGroup Childs { get; set; }

        public ColorCountGroup Counters { get; set; }

        public ImportanceColor HighImportanceColor
        {
            get
            {
                var colors = new[]
                {
                    Events.HighImportanceColor,
                    Checks.HighImportanceColor,
                    Childs.HighImportanceColor
                };
                return colors.OrderByDescending(x => x).First();
            }
        }
    }
}