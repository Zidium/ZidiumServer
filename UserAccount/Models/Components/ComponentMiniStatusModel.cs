using System;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Models
{
    public class ComponentMiniStatusModel
    {
        public Guid ComponentId { get; set; }

        public int Alarm { get; set; }

        public int Warning { get; set; }

        public int Success { get; set; }

        public int Unknown { get; set; }

        public string AlarmUrl { get; set; }

        public string WarningUrl { get; set; }

        public string SuccessUrl { get; set; }

        public string UnknownUrl { get; set; }

        public ImportanceColor MostImportantColor
        {
            get
            {
                if (Alarm > 0)
                    return ImportanceColor.Red;
                if (Warning > 0)
                    return ImportanceColor.Yellow;
                if (Success > 0)
                    return ImportanceColor.Green;
                return ImportanceColor.Gray;
            }
        }

        public int MostImportantCount
        {
            get
            {
                if (Alarm > 0)
                    return Alarm;
                if (Warning > 0)
                    return Warning;
                if (Success > 0)
                    return Success;
                return Unknown;
            }
        }
    }
}