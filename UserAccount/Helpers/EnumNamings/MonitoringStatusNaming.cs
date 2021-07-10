using Zidium.Api.Dto;

namespace Zidium.UserAccount.Helpers
{
    public class MonitoringStatusNaming : IEnumNaming<MonitoringStatus>
    {
        public string Name(MonitoringStatus value)
        {
            if (value == MonitoringStatus.Success)
                return "Success - Всё хорошо";

            if (value == MonitoringStatus.Warning)
                return "Warning - Требует внимания";

            if (value == MonitoringStatus.Alarm)
                return "Alarm - Необходимо принять меры";

            if (value == MonitoringStatus.Disabled)
                return "Disabled - Отключен";

            if (value == MonitoringStatus.Unknown)
                return "Unknown - Нет актуальных данных";

            return value.ToString();
        }
    }
}