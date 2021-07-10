using System;
using System.Linq;
using Zidium.Api.Dto;
using Zidium.Storage;
using Zidium.UserAccount.Controllers;
using Zidium.UserAccount.Models;

namespace Zidium.UserAccount.Helpers
{
    public static class ComponentMiniStatusHelper
    {
        public static ComponentMiniStatusModel GetEventsMiniStatusModel(this BaseController controller, Guid id, GetGuiComponentShowInfo component)
        {
            var now = controller.Now();
            var actualEvents = component.ActualEventsMiniInfo;

            var alarmEvents = actualEvents.Where(t => t.Importance == EventImportance.Alarm).ToArray();
            var warningEvents = actualEvents.Where(t => t.Importance == EventImportance.Warning).ToArray();
            var successEvents = actualEvents.Where(t => t.Importance == EventImportance.Success).ToArray();
            var unknownEvents = actualEvents.Where(t => t.Importance == EventImportance.Unknown).ToArray();

            var eventsMiniStatus = new ComponentMiniStatusModel()
            {
                ComponentId = id,
                Alarm = alarmEvents.Length,
                Warning = warningEvents.Length,
                Success = successEvents.Length,
                Unknown = unknownEvents.Length,
                AlarmUrl = ColorLinkHelper.GetEventsUrl(controller.Url, id, now, ObjectColor.Red, alarmEvents.Select(t => t.Id).ToArray()),
                WarningUrl = ColorLinkHelper.GetEventsUrl(controller.Url, id, now, ObjectColor.Yellow, warningEvents.Select(t => t.Id).ToArray()),
                SuccessUrl = ColorLinkHelper.GetEventsUrl(controller.Url, id, now, ObjectColor.Green, successEvents.Select(t => t.Id).ToArray()),
                UnknownUrl = ColorLinkHelper.GetEventsUrl(controller.Url, id, now, ObjectColor.Gray, unknownEvents.Select(t => t.Id).ToArray()),
            };

            return eventsMiniStatus;
        }

        public static ComponentMiniStatusModel GetUnittestsMiniStatusModel(this BaseController controller, Guid id, GetGuiComponentShowInfo component)
        {
            var unitTests = component.UnitTestsMiniInfo;

            var alarmUnitTests = unitTests.Where(t => t.Status == MonitoringStatus.Alarm).ToArray();
            var warningUnitTests = unitTests.Where(t => t.Status == MonitoringStatus.Warning).ToArray();
            var successUnitTests = unitTests.Where(t => t.Status == MonitoringStatus.Success).ToArray();
            var unknownUnitTests = unitTests.Where(t => t.Status == MonitoringStatus.Unknown || t.Status == MonitoringStatus.Disabled).ToArray();

            var checksMiniStatus = new ComponentMiniStatusModel()
            {
                ComponentId = id,
                Alarm = alarmUnitTests.Length,
                AlarmUrl = ColorLinkHelper.GetUnitTestsUrl(controller.Url, id, ObjectColor.Red, alarmUnitTests.Select(t => t.Id).ToArray()),
                Warning = warningUnitTests.Length,
                WarningUrl = ColorLinkHelper.GetUnitTestsUrl(controller.Url, id, ObjectColor.Yellow, warningUnitTests.Select(t => t.Id).ToArray()),
                Success = successUnitTests.Length,
                SuccessUrl = ColorLinkHelper.GetUnitTestsUrl(controller.Url, id, ObjectColor.Green, successUnitTests.Select(t => t.Id).ToArray()),
                Unknown = unknownUnitTests.Length,
                UnknownUrl = ColorLinkHelper.GetUnitTestsUrl(controller.Url, id, ObjectColor.Gray, unknownUnitTests.Select(t => t.Id).ToArray())
            };

            return checksMiniStatus;
        }

        public static ComponentMiniStatusModel GetMetricsMiniStatusModel(this BaseController controller, Guid id, GetGuiComponentShowInfo component)
        {
            var metrics = component.MetricsMiniInfo;

            var alarmMetrics = metrics.Where(t => t.Status == MonitoringStatus.Alarm).ToArray();
            var warningMetrics = metrics.Where(t => t.Status == MonitoringStatus.Warning).ToArray();
            var successMetrics = metrics.Where(t => t.Status == MonitoringStatus.Success).ToArray();
            var unknownMetrics = metrics.Where(t => t.Status == MonitoringStatus.Unknown || t.Status == MonitoringStatus.Disabled).ToArray();

            var metricsMiniStatus = new ComponentMiniStatusModel()
            {
                ComponentId = id,
                Alarm = alarmMetrics.Length,
                Warning = warningMetrics.Length,
                Success = successMetrics.Length,
                Unknown = unknownMetrics.Length,
                AlarmUrl = ColorLinkHelper.GetMetricsUrl(controller.Url, id, ObjectColor.Red, alarmMetrics.Select(t => t.Id).ToArray()),
                WarningUrl = ColorLinkHelper.GetMetricsUrl(controller.Url, id, ObjectColor.Yellow, warningMetrics.Select(t => t.Id).ToArray()),
                SuccessUrl = ColorLinkHelper.GetMetricsUrl(controller.Url, id, ObjectColor.Green, successMetrics.Select(t => t.Id).ToArray()),
                UnknownUrl = ColorLinkHelper.GetMetricsUrl(controller.Url, id, ObjectColor.Gray, unknownMetrics.Select(t => t.Id).ToArray()),
            };

            return metricsMiniStatus;
        }

        public static ComponentMiniStatusModel GetChildsMiniStatusModel(this BaseController controller, Guid id, GetGuiComponentShowInfo component)
        {
            var childs = component.ChildsMiniInfo;

            var alarmChilds = childs.Where(t => t.Status == MonitoringStatus.Alarm).ToArray();
            var warningChilds = childs.Where(t => t.Status == MonitoringStatus.Warning).ToArray();
            var successChilds = childs.Where(t => t.Status == MonitoringStatus.Success).ToArray();
            var unknownChilds = childs.Where(t => t.Status == MonitoringStatus.Unknown || t.Status == MonitoringStatus.Disabled).ToArray();

            var childsMiniStatus = new ComponentMiniStatusModel()
            {
                ComponentId = id,
                Alarm = alarmChilds.Length,
                Warning = warningChilds.Length,
                Success = successChilds.Length,
                Unknown = unknownChilds.Length,
                AlarmUrl = ColorLinkHelper.GetChildsUrl(controller.Url, id, ObjectColor.Red, alarmChilds.Select(t => t.Id).ToArray()),
                WarningUrl = ColorLinkHelper.GetChildsUrl(controller.Url, id, ObjectColor.Yellow, warningChilds.Select(t => t.Id).ToArray()),
                SuccessUrl = ColorLinkHelper.GetChildsUrl(controller.Url, id, ObjectColor.Green, successChilds.Select(t => t.Id).ToArray()),
                UnknownUrl = ColorLinkHelper.GetChildsUrl(controller.Url, id, ObjectColor.Gray, unknownChilds.Select(t => t.Id).ToArray()),
            };

            return childsMiniStatus;
        }

    }
}
