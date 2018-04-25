using System.Collections.Generic;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Models.CheckModels
{
    public class ResultsModel
    {
        public List<UnitTest> Tests { get; set; }

        public ImportanceColor GetColor(UnitTest unitTest)
        {
            if (unitTest.Bulb.Status == MonitoringStatus.Alarm)
            {
                return ImportanceColor.Red;
            }
            if (unitTest.Bulb.Status == MonitoringStatus.Warning)
            {
                return ImportanceColor.Yellow;
            }
            if (unitTest.Bulb.Status == MonitoringStatus.Success)
            {
                return ImportanceColor.Green;
            }
            if (unitTest.Bulb.Status == MonitoringStatus.Unknown)
            {
                return ImportanceColor.Gray;
            }
            return ImportanceColor.Gray;
        }
    }
}