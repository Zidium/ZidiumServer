using System;
using System.Linq;
using System.Text;
using System.Threading;
using NLog;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;

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
            ILogger logger,
            Notification notification,
            AccountDbContext accountDbContext,
            string accountName,
            Guid accountId)
        {
            var componentRepository = accountDbContext.GetComponentRepository();
            var component = componentRepository.GetById(notification.Event.OwnerId);

            // составляем тело письма
            var body = GetStatusEventText(logger, notification, component, accountDbContext, accountName);

            // сохраняем письмо в очередь
            var messageCommandRepository = accountDbContext.GetSendMessageCommandRepository();
            var command = new SendMessageCommand()
            {
                Id = Guid.NewGuid(),
                Body = body,
                To = notification.Address,
                Channel = notification.Type,
                ReferenceId = notification.Id
            };
            messageCommandRepository.Add(command);

            // заполним ссылку на письмо в уведомлении
            notification.SendMessageCommand = command;
        }

        protected Event GetRecentReasonEvent(Event statusEvent, AccountDbContext accountDbContext)
        {
            var repository = accountDbContext.GetEventRepository();
            return repository.GetRecentReasonEvent(statusEvent);
        }

        protected string GetStatusEventText(ILogger logger, Notification notification, Component component, AccountDbContext accountDbContext, string accountName)
        {
            var statusEvent = notification.Event;
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

            var componentPathText = ComponentHelper.GetComponentPathText(component);
            text.AppendLine(componentPathText);

            // Причину показываем, только если статус стал опаснее
            if (statusEvent.ImportanceHasGrown)
            {
                var reasonEvent = GetRecentReasonEvent(statusEvent, accountDbContext);

                if (reasonEvent != null)
                {
                    text.AppendLine("Причина: ");

                    // причина - ошибка компонента
                    if (reasonEvent.Category == EventCategory.ApplicationError)
                    {
                        var eventTypeRepository = accountDbContext.GetEventTypeRepository();
                        var errorType = eventTypeRepository.GetById(reasonEvent.EventTypeId);
                        var errorTitle = "Ошибка " + (errorType.Code ?? errorType.DisplayName);
                        text.AppendLine(errorTitle);
                        var urlText = UrlHelper.GetEventUrl(reasonEvent.Id, accountName);
                        text.AppendLine(urlText);
                    }

                    // причина - событие компонента
                    if (reasonEvent.Category == EventCategory.ComponentEvent)
                    {
                        var eventTypeRepository = accountDbContext.GetEventTypeRepository();
                        var componentEventType = eventTypeRepository.GetById(reasonEvent.EventTypeId);
                        var errorTitle = "Событие компонента " + componentEventType.DisplayName;
                        text.AppendLine(errorTitle);
                        var urlText = UrlHelper.GetEventUrl(reasonEvent.Id, accountName);
                        text.AppendLine(urlText);
                    }

                    // причина - статус дочернего компонента
                    if (reasonEvent.Category == EventCategory.ComponentExternalStatus)
                    {
                        var componentRepository = accountDbContext.GetComponentRepository();
                        var childComponent = componentRepository.GetById(reasonEvent.OwnerId);
                        var errorTitle = "Статус " + statusName + " дочернего компонента " + childComponent.DisplayName;
                        text.AppendLine(errorTitle);
                        var urlText = UrlHelper.GetComponentUrl(childComponent.Id, accountName);
                        text.AppendLine(urlText);
                    }

                    // причина - результат проверки
                    if (reasonEvent.Category == EventCategory.UnitTestResult)
                    {
                        var unitTestRepository = accountDbContext.GetUnitTestRepository();
                        var unitTest = unitTestRepository.GetById(reasonEvent.OwnerId);
                        var errorTitle = "Результат проверки " + unitTest.DisplayName;
                        text.AppendLine(errorTitle);
                        var urlText = UrlHelper.GetUnitTestUrl(unitTest.Id, accountName);
                        text.AppendLine(urlText);
                    }

                    // причина - значение метрики
                    if (reasonEvent.Category == EventCategory.MetricStatus)
                    {
                        var metricRepository = accountDbContext.GetMetricRepository();
                        var metric = metricRepository.GetById(reasonEvent.OwnerId);
                        var metricHistoryRepository = accountDbContext.GetMetricHistoryRepository();

                        var firstValue = metricHistoryRepository.GetAllByStatus(reasonEvent.Id)
                                .OrderBy(x => x.BeginDate)
                                .FirstOrDefault();
                        var firstValueText = firstValue != null ? firstValue.Value.ToString() : "null";

                        var errorTitle = "Значение метрики " + metric.MetricType.DisplayName + " = " + firstValueText;
                        text.AppendLine(errorTitle);
                        var urlText = UrlHelper.GetMetricUrl(metric.Id, accountName);
                        text.AppendLine(urlText);
                    }

                }
            }
            else
            {
                var urlText = UrlHelper.GetComponentUrl(component.Id, accountName);
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