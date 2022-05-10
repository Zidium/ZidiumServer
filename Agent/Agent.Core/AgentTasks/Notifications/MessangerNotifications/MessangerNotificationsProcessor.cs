using System;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks.Notifications
{
    /// <summary>
    /// Для каждого уведомления (с типами мессенджеров) создает команду на отправку
    /// </summary>
    public class MessangerNotificationsProcessor : NotificationSenderBase
    {
        public MessangerNotificationsProcessor(ILogger logger, CancellationToken cancellationToken)
            : base(logger, cancellationToken) { }

        protected override SubscriptionChannel[] Channels { get; } = new[]
        {
            SubscriptionChannel.Telegram,
            SubscriptionChannel.VKontakte
        };

        protected override void Send(
            NotificationForRead notification,
            IStorage storage,
            NotificationForUpdate notificationForUpdate)
        {
            var eventObj = storage.Events.GetOneById(notification.EventId);
            var component = storage.Components.GetOneById(eventObj.OwnerId);

            // составляем тело письма
            var body = GetStatusEventText(notification, component, eventObj, storage);

            // сохраняем письмо в очередь
            var command = new SendMessageCommandForAdd()
            {
                Id = Ulid.NewUlid(),
                Status = MessageStatus.InQueue,
                CreateDate = TimeService.Now(),
                Body = body,
                To = notification.Address,
                Channel = notification.Type,
                ReferenceId = notification.Id
            };
            storage.SendMessageCommands.Add(command);

            // заполним ссылку на письмо в уведомлении
            notificationForUpdate.SendMessageCommandId.Set(command.Id);
        }

        protected EventForRead GetRecentReasonEvent(EventForRead statusEvent, IStorage storage)
        {
            var service = new EventService(storage, TimeService);
            return service.GetRecentReasonEvent(statusEvent);
        }

        protected string GetStatusEventText(
            NotificationForRead notification,
            ComponentForRead component,
            EventForRead statusEvent,
            IStorage storage)
        {
            var text = new StringBuilder();

            // Делаем строку:
            // ОШИБКА - ASMO/ПДК-10

            if (notification.Reason == NotificationReason.Reminder)
            {
                text.AppendLine("(напоминание)");
            }

            var eventImportance = statusEvent.Importance;
            var statusName = GetEventImportanceText(eventImportance);
            text.Append(statusName);

            text.Append(" - ");

            var componentPathText = ComponentHelper.GetComponentPathText(component, storage);
            text.AppendLine(componentPathText);

            // Причину показываем, только если статус стал опаснее
            if (statusEvent.ImportanceHasGrown())
            {
                var reasonEvent = GetRecentReasonEvent(statusEvent, storage);

                if (reasonEvent != null)
                {
                    text.AppendLine("Причина: ");

                    // причина - ошибка компонента
                    if (reasonEvent.Category == EventCategory.ApplicationError)
                    {
                        var errorType = storage.EventTypes.GetOneById(reasonEvent.EventTypeId);
                        var errorTitle = "Ошибка " + (errorType.Code ?? errorType.DisplayName);
                        text.AppendLine(errorTitle);
                        var urlText = UrlHelper.GetEventUrl(reasonEvent.Id);
                        text.AppendLine(urlText);
                    }

                    // причина - событие компонента
                    if (reasonEvent.Category == EventCategory.ComponentEvent)
                    {
                        var componentEventType = storage.EventTypes.GetOneById(reasonEvent.EventTypeId);
                        var errorTitle = "Событие компонента " + componentEventType.DisplayName;
                        text.AppendLine(errorTitle);
                        var urlText = UrlHelper.GetEventUrl(reasonEvent.Id);
                        text.AppendLine(urlText);
                    }

                    // причина - статус дочернего компонента
                    if (reasonEvent.Category == EventCategory.ComponentExternalStatus)
                    {
                        var childComponent = storage.Components.GetOneById(reasonEvent.OwnerId);
                        var errorTitle = "Статус " + statusName + " дочернего компонента " + childComponent.DisplayName;
                        text.AppendLine(errorTitle);
                        var urlText = UrlHelper.GetComponentUrl(childComponent.Id);
                        text.AppendLine(urlText);
                    }

                    // причина - результат проверки
                    if (reasonEvent.Category == EventCategory.UnitTestResult)
                    {
                        var unitTest = storage.UnitTests.GetOneById(reasonEvent.OwnerId);
                        var errorTitle = "Результат проверки " + unitTest.DisplayName;
                        text.AppendLine(errorTitle);
                        var urlText = UrlHelper.GetUnitTestUrl(unitTest.Id);
                        text.AppendLine(urlText);
                    }

                    // причина - значение метрики
                    if (reasonEvent.Category == EventCategory.MetricStatus)
                    {
                        var metric = storage.Metrics.GetOneById(reasonEvent.OwnerId);
                        var metricType = storage.MetricTypes.GetOneById(metric.MetricTypeId);

                        var firstValue = storage.MetricHistory.GetFirstByStatusEventId(reasonEvent.Id);
                        var firstValueText = firstValue != null ? firstValue.Value.ToString() : "null";

                        var errorTitle = "Значение метрики " + metricType.DisplayName + " = " + firstValueText;
                        text.AppendLine(errorTitle);
                        var urlText = UrlHelper.GetMetricUrl(metric.Id);
                        text.AppendLine(urlText);
                    }
                }
            }
            else
            {
                var urlText = UrlHelper.GetComponentUrl(component.Id);
                text.AppendLine(urlText);
            }

            var body = text.ToString();
            return body;
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