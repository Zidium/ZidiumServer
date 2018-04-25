using System;
using System.Linq;
using System.Text;
using System.Threading;
using NLog;
using Zidium.Api;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Api.Dispatcher;
using Zidium.Core.Common.Helpers;

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

        protected override NotificationType NotificationType
        {
            get { return NotificationType.Sms; }
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

            var eventImportance = statusEvent.Importance;
            var statusName = GetEventImportanceText(eventImportance);
            text.Append(statusName);

            text.Append(" - ");

            var componentPathText = ComponentHelper.GetComponentPathText(component);
            text.Append(componentPathText);

            // Причину показываем, только если статус стал опаснее
            if (statusEvent.ImportanceHasGrown)
            {
                var reasonEvent = GetRecentReasonEvent(statusEvent, accountDbContext);

                if (reasonEvent != null)
                {
                    text.Append(", причина: ");

                    // причина - ошибка компонента
                    if (reasonEvent.Category == Core.Api.EventCategory.ApplicationError)
                    {
                        var eventTypeRepository = accountDbContext.GetEventTypeRepository();
                        var errorType = eventTypeRepository.GetById(reasonEvent.EventTypeId);
                        var errorTitle = "ошибка " + (errorType.Code != null ? errorType.Code : errorType.DisplayName);
                        text.Append(errorTitle);
                    }

                    // причина - событие компонента
                    if (reasonEvent.Category == Core.Api.EventCategory.ComponentEvent)
                    {
                        var eventTypeRepository = accountDbContext.GetEventTypeRepository();
                        var componentEventType = eventTypeRepository.GetById(reasonEvent.EventTypeId);
                        var errorTitle = "событие компонента " + componentEventType.DisplayName;
                        text.Append(errorTitle);
                    }

                    // причина - статус дочернего компонента
                    if (reasonEvent.Category == Core.Api.EventCategory.ComponentExternalStatus)
                    {
                        var componentRepository = accountDbContext.GetComponentRepository();
                        var childComponent = componentRepository.GetById(reasonEvent.OwnerId);
                        var errorTitle = "статус " + statusName + " дочернего компонента " + childComponent.DisplayName;
                        text.Append(errorTitle);
                    }

                    // причина - результат проверки
                    if (reasonEvent.Category == Core.Api.EventCategory.UnitTestResult)
                    {
                        var unitTestRepository = accountDbContext.GetUnitTestRepository();
                        var unitTest = unitTestRepository.GetById(reasonEvent.OwnerId);
                        var errorTitle = "результат проверки " + unitTest.DisplayName;
                        text.Append(errorTitle);
                    }

                    // причина - значение метрики
                    if (reasonEvent.Category == Core.Api.EventCategory.MetricStatus)
                    {
                        var metricRepository = accountDbContext.GetMetricRepository();
                        var metric = metricRepository.GetById(reasonEvent.OwnerId);
                        var metricHistoryRepository = accountDbContext.GetMetricHistoryRepository();

                        var firstValue = metricHistoryRepository.GetAllByStatus(reasonEvent.Id)
                                .OrderBy(x => x.BeginDate)
                                .FirstOrDefault();
                        var firstValueText = firstValue != null ? firstValue.Value.ToString() : "null";

                        var errorTitle = "значение метрики " + metric.MetricType.DisplayName + " = " + firstValueText;
                        text.Append(errorTitle);
                    }
                }
            }

            var body = text.ToString();
            return body;
        }

        protected override void Send(ILogger logger,
            Notification notification,
            AccountDbContext accountDbContext,
            string accountName, Guid accountId)
        {
            var componentRepository = accountDbContext.GetComponentRepository();
            var component = componentRepository.GetById(notification.Event.OwnerId);
            var body = GetStatusEventText(logger, notification, component, accountDbContext, accountName);

            var data = new SendSmsRequestData()
            {
                Phone = notification.Address,
                Body = body,
                ReferenceId = notification.Id
            };
            var response = Dispatcher.SendSms(accountId, data);

            // Лимит на количество SMS игнорируем - это нормальная ситуация
            if (!response.Success && response.Code == ResponseCode.OverLimit)
            {
                return;
            }

            response.Check();
        }

        protected string GetEventImportanceText(Core.Api.EventImportance importance)
        {
            if (importance == Core.Api.EventImportance.Alarm)
            {
                return "ОШИБКА";
            }

            if (importance == Core.Api.EventImportance.Warning)
            {
                return "ПРЕДУПРЕЖДЕНИЕ";
            }

            if (importance == Core.Api.EventImportance.Success)
            {
                return "ВСЁ ХОРОШО";
            }

            return "НЕИЗВЕСТНО";
        }
    }
}
