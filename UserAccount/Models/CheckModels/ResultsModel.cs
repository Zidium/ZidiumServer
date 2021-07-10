using Zidium.Api.Dto;
using Zidium.Storage;
using Zidium.UserAccount.Models.Controls;

namespace Zidium.UserAccount.Models.CheckModels
{
    public class ResultsModel
    {
        public GetGuiChecksResultsInfo[] Tests { get; set; }

        public ImportanceColor GetColor(GetGuiChecksResultsInfo unitTest)
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