using Zidium.Storage;

namespace Zidium.UserAccount.Helpers
{
    public class EventImportanceNaming : IEnumNaming<EventImportance>
    {
        public string Name(EventImportance value)
        {
            if (value == EventImportance.Alarm)
                return "Alarm (необходимо принять меры)";

            if (value == EventImportance.Warning)
                return "Warning (предупреждение)";

            if (value == EventImportance.Success)
                return "Info (все хорошо)";

            if (value == EventImportance.Unknown)
                return "Unknown (не меняет статус компонента)";

            return "Неизвестная важность";
        }
    }
}