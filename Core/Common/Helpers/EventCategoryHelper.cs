using System.Linq;
using Zidium.Core.Api;

namespace Zidium.Core
{
    public static class EventCategoryHelper
    {
        public static readonly EventCategory[] AllCategories =
            new[]
            {
                EventCategory.ComponentEvent,
                EventCategory.ApplicationError,
                EventCategory.UnitTestResult,
                EventCategory.UnitTestStatus,
                EventCategory.MetricStatus,
                EventCategory.ComponentUnitTestsStatus,
                EventCategory.ComponentEventsStatus,
                EventCategory.ComponentMetricsStatus,
                EventCategory.ComponentChildsStatus,
                EventCategory.ComponentInternalStatus,
                EventCategory.ComponentExternalStatus
            };

        public static readonly EventCategory[] ComponentCategories =
            new[]
            {
                EventCategory.ComponentEvent,
                EventCategory.ApplicationError,
                EventCategory.ComponentUnitTestsStatus,
                EventCategory.ComponentEventsStatus,
                EventCategory.ComponentMetricsStatus,
                EventCategory.ComponentChildsStatus,
                EventCategory.ComponentInternalStatus,
                EventCategory.ComponentExternalStatus
            };

        public static readonly EventCategory[] StatusCategories =
            new[]
            {
                EventCategory.ComponentExternalStatus,
                EventCategory.ComponentInternalStatus,
                EventCategory.ComponentChildsStatus,
                EventCategory.ComponentEventsStatus,
                EventCategory.ComponentMetricsStatus,
                EventCategory.ComponentUnitTestsStatus,
                EventCategory.MetricStatus,
                EventCategory.UnitTestStatus,
                EventCategory.UnitTestResult
            };

        public static readonly EventCategory[] CustomerCategories =
            new[]
            {
                EventCategory.ApplicationError,
                EventCategory.ComponentEvent
            };

        public static bool IsComponentCategory(this EventCategory category)
        {
            return ComponentCategories.Contains(category);
        }

        public static bool IsStatus(this EventCategory category)
        {
            return StatusCategories.Contains(category);
        }

        public static bool IsUnitTestCategory(this EventCategory category)
        {
            return
                category == EventCategory.UnitTestResult ||
                category == EventCategory.UnitTestStatus;
        }

        public static bool IsMetricCategory(this EventCategory category)
        {
            return category == EventCategory.MetricStatus;
        }

        public static bool IsCustomerEventCategory(EventCategory category)
        {
            return CustomerCategories.Contains(category);
        }
    }
}
