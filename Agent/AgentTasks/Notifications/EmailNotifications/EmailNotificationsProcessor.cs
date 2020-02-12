using System;
using System.Linq;
using System.Threading;
using NLog;
using Zidium.Core.Common;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common.Helpers;

namespace Zidium.Agent.AgentTasks.Notifications
{
    /// <summary>
    /// Для каждого уведомления (с типом email) создает команду на отправку email
    /// </summary>
    public class EmailNotificationsProcessor : NotificationSenderBase
    {
        public EmailNotificationsProcessor(ILogger logger, CancellationToken cancellationToken)
            : base(logger, cancellationToken) { }

        protected override SubscriptionChannel[] Channels { get; } = new[] { SubscriptionChannel.Email };

        protected override void Send(
            ILogger logger,
            Notification notification,
            AccountDbContext accountDbContext,
            string accountName, 
            Guid accountId)
        {
            var componentRepository = accountDbContext.GetComponentRepository();
            var component = componentRepository.GetById(notification.Event.OwnerId);
            var path = ComponentHelper.GetComponentPathText(component);

            var subject = string.Format("{0} - {1}", path, GetEventImportanceText(notification.Event.Importance));
            if (notification.Reason == NotificationReason.Reminder)
            {
                subject += " (напоминание)";
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
                To = notification.Address,
                ReferenceId = notification.Id
            };
            emailRepository.Add(command);

            // заполним ссылку на письмо в уведомлении
            notification.SendEmailCommand = command;
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

            var statusTimeText = string.Format(
                "{0} в {1}",
                statusEvent.StartDate.ToString("dd.MM.yyyy"),
                statusEvent.StartDate.ToString("HH:mm:ss"));

            html.Write(statusTimeText);
            html.Write(" компонент ");

            var componentPathText = ComponentHelper.GetComponentPathText(component);
            html.Write(componentPathText);

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

                    // причина - ошибка компонента
                    if (reasonEvent.Category == EventCategory.ApplicationError)
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

                        var reasonEventUrl = UrlHelper.GetEventUrl(reasonEvent.Id, accountName);
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
                    if (reasonEvent.Category == EventCategory.ComponentEvent)
                    {
                        // Делаем строку:
                        // Причиной стало событие компонента Запуск компонента: Данный ключ отсутствует в словаре
                        html.Write("Причиной стало ");

                        var eventTypeRepository = accountDbContext.GetEventTypeRepository();
                        var componentEventType = eventTypeRepository.GetById(reasonEvent.EventTypeId);

                        var reasonEventUrl = UrlHelper.GetEventUrl(reasonEvent.Id, accountName);
                        html.WriteLink(reasonEventUrl, " событие " + componentEventType.DisplayName);

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
                    if (reasonEvent.Category == EventCategory.ComponentExternalStatus)
                    {
                        var componentRepository = accountDbContext.GetComponentRepository();
                        var childComponent = componentRepository.GetById(reasonEvent.OwnerId);

                        // Делаем строку:
                        // Причиной стал статус ОШИБКА дочернего компонента ВИДЕОКАМЕРА

                        html.Write("Причиной стал ");
                        var title = "статус " + statusName + " дочернего компонента " + childComponent.DisplayName;

                        var reasonEventUrl = UrlHelper.GetComponentUrl(childComponent.Id, accountName);
                        html.WriteLink(reasonEventUrl, title);
                    }

                    // причина - результат проверки
                    if (reasonEvent.Category == EventCategory.UnitTestResult)
                    {
                        var unitTestRepository = accountDbContext.GetUnitTestRepository();
                        var unitTest = unitTestRepository.GetById(reasonEvent.OwnerId);

                        // Делаем строку:
                        // Причиной стал РЕЗУЛЬТАТ проверки МОЯ ПРОВЕРКА: Превышен таймаут выполнения запроса

                        html.Write("Причиной стал ");
                        var title = "результат проверки " + unitTest.DisplayName;

                        var reasonEventUrl = UrlHelper.GetUnitTestUrl(unitTest.Id, accountName);
                        html.WriteLink(reasonEventUrl, title);

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
                    if (reasonEvent.Category == EventCategory.MetricStatus)
                    {
                        var metricRepository = accountDbContext.GetMetricRepository();
                        var metric = metricRepository.GetById(reasonEvent.OwnerId);
                        var metricHistoryRepository = accountDbContext.GetMetricHistoryRepository();

                        var firstValue = metricHistoryRepository.GetAllByStatus(reasonEvent.Id)
                                .OrderBy(x => x.BeginDate)
                                .FirstOrDefault();

                        var firstValueText = firstValue != null ? firstValue.Value.ToString() : "null";

                        // Делаем строку:
                        // Причиной стало значение метрики CPU, % = 200

                        html.Write("Причиной стало ");
                        var title = "значение метрики " + metric.MetricType.DisplayName;

                        var reasonEventUrl = UrlHelper.GetMetricUrl(metric.Id, accountName);
                        html.WriteLink(reasonEventUrl, title);

                        html.Write(" = ");
                        html.Span(firstValueText, "font-weight: bold, color: " + statusColor);
                    }
                }
            }
            else
            {
                var reasonEventUrl = UrlHelper.GetComponentUrl(component.Id, accountName);
                html.NewLine();
                html.WriteLink(reasonEventUrl, "Посмотреть подробности");
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

        protected static string EventImportanceToColor(EventImportance eventImportance)
        {
            if (eventImportance == EventImportance.Alarm)
                return "darkred";

            if (eventImportance == EventImportance.Warning)
                return "darkgoldenrod";

            if (eventImportance == EventImportance.Success)
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
