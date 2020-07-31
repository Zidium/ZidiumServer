using Zidium.Storage;

namespace Zidium.UserAccount.Helpers
{
    public class EventCategoryNaming : IEnumNaming<EventCategory>
    {
        public string Name(EventCategory value)
        {
            if (value == EventCategory.ComponentEvent)
                return "ComponentEvent - Событие компонента";

            if (value == EventCategory.ApplicationError)
                return "ApplicationError - Ошибка приложения";

            if (value == EventCategory.UnitTestResult)
                return "UnitTestResult - Результат проверки";

            if (value == EventCategory.UnitTestStatus)
                return "UnitTestStatus - Статус проверки";

            if (value == EventCategory.MetricStatus)
                return "MetricStatus - Статус метрики";

            if (value == EventCategory.ComponentUnitTestsStatus)
                return "ComponentUnitTestsStatus - Статус проверок";

            if (value == EventCategory.ComponentEventsStatus)
                return "ComponentEventsStatus - Статус событий";

            if (value == EventCategory.ComponentMetricsStatus)
                return "ComponentMetricsStatus - Статус метрик";

            if (value == EventCategory.ComponentChildsStatus)
                return "ComponentChildsStatus - Статус детей";

            if (value == EventCategory.ComponentInternalStatus)
                return "ComponentInternalStatus - Внутренний статус";

            if (value == EventCategory.ComponentExternalStatus)
                return "ComponentExternalStatus - Итоговый статус";

            return value.ToString();
        }
    }
}