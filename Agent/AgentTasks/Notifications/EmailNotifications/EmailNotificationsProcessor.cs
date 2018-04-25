using System;
using System.Linq;
using System.Threading;
using NLog;
using Zidium.Api;
using Zidium.Core.Common;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common.Helpers;

namespace Zidium.Agent.AgentTasks.Notifications
{
    /// <summary>
    /// Для каждого уведомления (с типом email) создает задачу на отправку email
    /// </summary>
    public class EmailNotificationsProcessor : NotificationSenderBase
    {
        public EmailNotificationsProcessor(ILogger logger, CancellationToken cancellationToken)
            : base(logger, cancellationToken) { }

        protected override NotificationType NotificationType
        {
            get { return NotificationType.Email; }
        }

        protected Event GetRecentReasonEvent(Event statusEvent, AccountDbContext accountDbContext)
        {
            var repository = accountDbContext.GetEventRepository();
            return repository.GetRecentReasonEvent(statusEvent);
        }

        protected string GetStatusEventHtml(User user, ILogger logger, Notification notification, Component component, AccountDbContext accountDbContext, string accountName)
        {
            var statusEvent = notification.Event;
            var html = new HtmlRender();

            if (notification.Reason == NotificationReason.Reminder)
            {
                html.Span("Напоминаем, что ");
            }

            // Делаем строку:
            // 30.04.2016 в 08:37:43 компонент ПДК-10 перешёл в статус ОШИБКА.
            var statusUrl = UrlHelper.GetEventUrl(statusEvent.Id, accountName);

            var statusTimeText = string.Format(
                "{0} в {1}",
                statusEvent.StartDate.ToString("dd.MM.yyyy"),
                statusEvent.StartDate.ToString("HH:mm:ss"));

            html.WriteLink(statusUrl, statusTimeText);
            html.Write(" компонент ");

            var componentPathHtml = ComponentHelper.GetComponentPathHtml(component, accountName);
            html.WriteRaw(componentPathHtml);

            if (statusEvent.ImportanceHasGrown)
                html.Write(" перешёл в статус ");
            else
                html.Write(" вернулся в статус ");

            var eventImportance = statusEvent.Importance;
            var statusColor = EventImportanceToColor(eventImportance);
            var statusName = GetEventImportanceText(eventImportance);

            html.Span(statusName, "font-weight: bold; color: " + statusColor);
            html.Write(".");

            // Причину показываем, только если статус стал опаснее
            if (statusEvent.ImportanceHasGrown)
            {
                var reasonEvent = GetRecentReasonEvent(statusEvent, accountDbContext);

                if (reasonEvent != null)
                {
                    html.NewLine();
                    html.NewLine();

                    var reasonEventUrl = UrlHelper.GetEventUrl(reasonEvent.Id, accountName);

                    // причина - ошибка компонента
                    if (reasonEvent.Category == Core.Api.EventCategory.ApplicationError)
                    {
                        // Делаем строку:
                        // Причиной стала ошибка 5412584 - Данный ключ отсутствует в словаре
                        html.Write("Причиной стала ");

                        var eventTypeRepository = accountDbContext.GetEventTypeRepository();
                        var errorType = eventTypeRepository.GetById(reasonEvent.EventTypeId);
                        var errorTitle = "ошибка";

                        if (errorType.Code != null)
                        {
                            errorTitle += " " + errorType.Code;
                        }

                        errorTitle += " - " + errorType.DisplayName;

                        html.WriteLink(reasonEventUrl, errorTitle);

                        if (reasonEvent.Message != null)
                        {
                            html.Write(": " + reasonEvent.Message);
                        }

                        if (reasonEvent.Properties.Count > 0)
                        {
                            html.NewLine();
                            html.NewLine();
                            WriteEventPropertiesTable(html, reasonEvent, "Дополнительные свойства ошибки:");
                        }
                    }

                    // причина - событие компонента
                    if (reasonEvent.Category == Core.Api.EventCategory.ComponentEvent)
                    {
                        // Делаем строку:
                        // Причиной стало событие компонента Запуск компонента: Данный ключ отсутствует в словаре
                        html.Write("Причиной стало событие компонента ");

                        var eventTypeRepository = accountDbContext.GetEventTypeRepository();
                        var componentEventType = eventTypeRepository.GetById(reasonEvent.EventTypeId);
                        html.WriteLink(reasonEventUrl, componentEventType.DisplayName);
                        if (reasonEvent.Message != null)
                        {
                            html.Write(": " + reasonEvent.Message);
                        }
                        if (reasonEvent.Properties.Count > 0)
                        {
                            html.NewLine();
                            html.NewLine();
                            WriteEventPropertiesTable(html, reasonEvent, "Дополнительные свойства события компонента:");
                        }
                    }

                    // причина - статус дочернего компонента
                    if (reasonEvent.Category == Core.Api.EventCategory.ComponentExternalStatus)
                    {
                        var componentRepository = accountDbContext.GetComponentRepository();
                        var childComponent = componentRepository.GetById(reasonEvent.OwnerId);
                        var childComponentUrl = UrlHelper.GetComponentUrl(childComponent.Id, accountName);

                        // Делаем строку:
                        // Причиной стал статус ОШИБКА дочернего компонента ВИДЕОКАМЕРА

                        html.Write("Причиной стал ");
                        html.WriteLink(reasonEventUrl, "статус " + statusName);
                        html.Write(" дочернего компонента ");
                        html.WriteLink(childComponentUrl, childComponent.DisplayName);
                    }

                    // причина - результат проверки
                    if (reasonEvent.Category == Core.Api.EventCategory.UnitTestResult)
                    {
                        var unitTestRepository = accountDbContext.GetUnitTestRepository();
                        var unitTest = unitTestRepository.GetById(reasonEvent.OwnerId);
                        var unitTestUrl = UrlHelper.GetUnitTestUrl(unitTest.Id, accountName);

                        // Делаем строку:
                        // Причиной стал РЕЗУЛЬТАТ проверки МОЯ ПРОВЕРКА: Превышен таймаут выполнения запроса

                        html.Write("Причиной стал ");
                        html.WriteLink(reasonEventUrl, "результат");
                        html.Write(" проверки ");
                        html.WriteLink(unitTestUrl, unitTest.DisplayName);
                        if (reasonEvent.Message != null)
                        {
                            html.Write(": " + reasonEvent.Message);
                        }
                        if (reasonEvent.Properties.Count > 0)
                        {
                            html.NewLine();
                            html.NewLine();
                            WriteEventPropertiesTable(html, reasonEvent, "Дополнительные свойства результата проверки:");
                        }
                    }

                    // причина - значение метрики
                    if (reasonEvent.Category == Core.Api.EventCategory.MetricStatus)
                    {
                        var metricRepository = accountDbContext.GetMetricRepository();
                        var metric = metricRepository.GetById(reasonEvent.OwnerId);
                        var metricUrl = UrlHelper.GetMetricUrl(metric.Id, accountName);
                        var metricHistoryRepository = accountDbContext.GetMetricHistoryRepository();

                        var firstValue = metricHistoryRepository.GetAllByStatus(reasonEvent.Id)
                                .OrderBy(x => x.BeginDate)
                                .FirstOrDefault();

                        var firstValueText = firstValue != null ? firstValue.Value.ToString() : "null";

                        // Делаем строку:
                        // Причиной стало значение метрики CPU, % = 200

                        html.Write("Причиной стало значение метрики ");
                        html.WriteLink(metricUrl, metric.MetricType.DisplayName);
                        html.Write(" = ");
                        html.Span(firstValueText, "font-weight: bold, color: " + statusColor);
                    }
                }
            }

            if (notification.SubscriptionId.HasValue)
            {
                html.NewLine();
                html.NewLine();

                var changeSettingsUrl = UrlHelper.GetSubscriptionEditUrl(component, user, accountName);
                html.WriteLink(changeSettingsUrl, "Изменить настройки уведомлений");
                html.NewLine();
                html.Span("ID уведомления: " + notification.Id, "color: gray");
            }
            
            var htmlBody = html.GetHtml();
            return NotificationHelper.HtmlToLetter(htmlBody, user);
        }

        protected static string EventImportanceToColor(Core.Api.EventImportance eventImportance)
        {
            if (eventImportance == Core.Api.EventImportance.Alarm)
                return "darkred";

            if (eventImportance == Core.Api.EventImportance.Warning)
                return "darkgoldenrod";

            if (eventImportance == Core.Api.EventImportance.Success)
                return "darkgreen";

            return "gray";
        }

        protected void WriteEventPropertiesTable(HtmlRender html, Event eventObj, string header)
        {
            if (eventObj.Properties.Count > 0)
            {
                html.Write(header);
                html.NewLine();

                html.BeginTable();
                foreach (var param in eventObj.Properties)
                {
                    html.WriteExtentionPropertyRow(param.Name, param.Value, param.DataType, true);
                }
                html.EndTable();
            }
        }

        protected override void Send(ILogger logger,
            Notification notification,
            AccountDbContext accountDbContext,
            string accountName, Guid accountId)
        {
            var to = notification.Address;
            var componentRepository = accountDbContext.GetComponentRepository();
            var component = componentRepository.GetById(notification.Event.OwnerId);
            var path = ComponentHelper.GetComponentPathText(component);
            
            var subject = string.Format("{0} - {1}", path, GetEventImportanceText(notification.Event.Importance));
            if (notification.Reason == NotificationReason.Reminder)
            {
                subject = subject + " (напоминание)";
            }

            // составляем тело письма
            var user = accountDbContext.GetUserRepository().GetById(notification.UserId);
            var htmlBody = GetStatusEventHtml(user, logger, notification, component, accountDbContext, accountName);

            // сохраняем письмо в очередь
            var emailRepository = accountDbContext.GetSendEmailCommandRepository();
            var command = new SendEmailCommand()
            {
                Id = Guid.NewGuid(),
                Body = htmlBody,
                IsHtml = true,
                Subject = subject,
                To = to,
                ReferenceId = notification.Id
            };
            emailRepository.Add(command);
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
