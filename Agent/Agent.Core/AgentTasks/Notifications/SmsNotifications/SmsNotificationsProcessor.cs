using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;
using Zidium.Api.Dto;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Api.Dispatcher;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks.Notifications
{
    /// <summary>
    /// Для каждого уведомления (с типом sms) создает задачу на отправку sms
    /// </summary>
    public class SmsNotificationsProcessor : NotificationSenderBase
    {
        public SmsNotificationsProcessor(ILogger logger, CancellationToken cancellationToken)
            : base(logger, cancellationToken)
        {
            Dispatcher = AgentHelper.GetDispatcherClient();
        }

        protected DispatcherClient Dispatcher;

        protected override SubscriptionChannel[] Channels { get; } = new[] { SubscriptionChannel.Sms };

        protected EventForRead GetRecentReasonEvent(EventForRead statusEvent, IStorage storage)
        {
            var service = new EventService(storage);
            return service.GetRecentReasonEvent(statusEvent);
        }

        protected string GetStatusEventText(
            NotificationForRead notification,
            ComponentForRead component,
            IStorage storage,
            EventForRead statusEvent)
        {
            var text = new StringBuilder();

            // Делаем строку:
            // ОШИБКА - ASMO/ПДК-10

            var eventImportance = statusEvent.Importance;
            var statusName = GetEventImportanceText(eventImportance);
            text.Append(statusName);

            text.Append(" - ");

            var componentPathText = ComponentHelper.GetComponentPathText(component, storage);
            text.Append(componentPathText);

            // Причину показываем, только если статус стал опаснее
            if (statusEvent.ImportanceHasGrown())
            {
                var reasonEvent = GetRecentReasonEvent(statusEvent, storage);

                if (reasonEvent != null)
                {
                    text.Append(", причина: ");

                    // причина - ошибка компонента
                    if (reasonEvent.Category == EventCategory.ApplicationError)
                    {
                        var errorType = storage.EventTypes.GetOneById(reasonEvent.EventTypeId);
                        var errorTitle = "ошибка " + (errorType.Code != null ? errorType.Code : errorType.DisplayName);
                        text.Append(errorTitle);
                    }

                    // причина - событие компонента
                    if (reasonEvent.Category == EventCategory.ComponentEvent)
                    {
                        var componentEventType = storage.EventTypes.GetOneById(reasonEvent.EventTypeId);
                        var errorTitle = "событие компонента " + componentEventType.DisplayName;
                        text.Append(errorTitle);
                    }

                    // причина - статус дочернего компонента
                    if (reasonEvent.Category == EventCategory.ComponentExternalStatus)
                    {
                        var childComponent = storage.Components.GetOneById(reasonEvent.OwnerId);
                        var errorTitle = "статус " + statusName + " дочернего компонента " + childComponent.DisplayName;
                        text.Append(errorTitle);
                    }

                    // причина - результат проверки
                    if (reasonEvent.Category == EventCategory.UnitTestResult)
                    {
                        var unitTest = storage.UnitTests.GetOneById(reasonEvent.OwnerId);
                        var errorTitle = "результат проверки " + unitTest.DisplayName;
                        text.Append(errorTitle);
                    }

                    // причина - значение метрики
                    if (reasonEvent.Category == EventCategory.MetricStatus)
                    {
                        var metric = storage.Metrics.GetOneById(reasonEvent.OwnerId);
                        var metricType = storage.MetricTypes.GetOneById(metric.MetricTypeId);

                        var firstValue = storage.MetricHistory.GetFirstByStatusEventId(reasonEvent.Id);
                        var firstValueText = firstValue != null ? firstValue.Value.ToString() : "null";

                        var errorTitle = "значение метрики " + metricType.DisplayName + " = " + firstValueText;
                        text.Append(errorTitle);
                    }
                }
            }

            var body = text.ToString();
            return body;
        }

        protected override void Send(
            NotificationForRead notification,
            IStorage storage,
            NotificationForUpdate notificationForUpdate)
        {
            var eventObj = storage.Events.GetOneById(notification.EventId);
            var component = storage.Components.GetOneById(eventObj.OwnerId);
            var body = GetStatusEventText(notification, component, storage, eventObj);

            var data = new SendSmsRequestData()
            {
                Phone = notification.Address,
                Body = body,
                ReferenceId = notification.Id
            };

            var response = Dispatcher.SendSms(data);
            response.Check();
        }

        protected string GetEventImportanceText(EventImportance importance)
        {
            if (importance == EventImportance.Alarm)
            {
                return "ОШИБКА";
            }

            if (importance == EventImportance.Warning)
            {
                return "ПРЕДУПРЕЖДЕНИЕ";
            }

            if (importance == EventImportance.Success)
            {
                return "ВСЁ ХОРОШО";
            }

            return "НЕИЗВЕСТНО";
        }
    }
}
