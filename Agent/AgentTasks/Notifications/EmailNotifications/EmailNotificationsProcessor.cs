using System;
using System.Threading;
using NLog;
using Zidium.Core.Common;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

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

        protected override void Send(ILogger logger,
            NotificationForRead notification,
            IStorage storage,
            string accountName,
            Guid accountId, 
            NotificationForUpdate notificationForUpdate)
        {
            var eventObj = storage.Events.GetOneById(notification.EventId);
            var component = storage.Components.GetOneById(eventObj.OwnerId);
            var path = ComponentHelper.GetComponentPathText(component, storage);

            var subject = string.Format("{0} - {1}", path, GetEventImportanceText(eventObj.Importance));
            if (notification.Reason == NotificationReason.Reminder)
            {
                subject += " (напоминание)";
            }

            // составляем тело письма
            var user = storage.Users.GetOneById(notification.UserId);
            var htmlBody = GetStatusEventHtml(user, logger, notification, component, storage, accountName, eventObj);

            // сохраняем письмо в очередь
            var command = new SendEmailCommandForAdd()
            {
                Id = Guid.NewGuid(),
                Status = EmailStatus.InQueue,
                CreateDate = DateTime.Now,
                Body = htmlBody,
                IsHtml = true,
                Subject = subject,
                To = notification.Address,
                ReferenceId = notification.Id
            };
            storage.SendEmailCommands.Add(command);

            // заполним ссылку на письмо в уведомлении
            notificationForUpdate.SendEmailCommandId.Set(command.Id);
        }

        protected EventForRead GetRecentReasonEvent(EventForRead statusEvent, IStorage storage)
        {
            var service = new EventService(storage);
            return service.GetRecentReasonEvent(statusEvent);
        }

        protected string GetStatusEventHtml(
            UserForRead user, 
            ILogger logger, 
            NotificationForRead notification, 
            ComponentForRead component, 
            IStorage storage, 
            string accountName,
            EventForRead statusEvent)
        {
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

            var componentPathText = ComponentHelper.GetComponentPathText(component, storage);
            html.Write(componentPathText);

            if (statusEvent.ImportanceHasGrown())
                html.Write(" перешёл в статус ");
            else
                html.Write(" вернулся в статус ");

            var eventImportance = statusEvent.Importance;
            var statusColor = EventImportanceToColor(eventImportance);
            var statusName = GetEventImportanceText(eventImportance);

            html.Span(statusName, "font-weight: bold; color: " + statusColor);
            html.Write(".");

            // Причину показываем, только если статус стал опаснее
            if (statusEvent.ImportanceHasGrown())
            {
                var reasonEvent = GetRecentReasonEvent(statusEvent, storage);

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

                        var errorType = storage.EventTypes.GetOneById(reasonEvent.EventTypeId);
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

                        var reasonEventProperties = storage.EventProperties.GetByEventId(reasonEvent.Id);
                        if (reasonEventProperties.Length > 0)
                        {
                            html.NewLine();
                            html.NewLine();
                            WriteEventPropertiesTable(html, reasonEventProperties, "Дополнительные свойства ошибки:");
                        }
                    }

                    // причина - событие компонента
                    if (reasonEvent.Category == EventCategory.ComponentEvent)
                    {
                        // Делаем строку:
                        // Причиной стало событие компонента Запуск компонента: Данный ключ отсутствует в словаре
                        html.Write("Причиной стало ");

                        var componentEventType = storage.EventTypes.GetOneById(reasonEvent.EventTypeId);

                        var reasonEventUrl = UrlHelper.GetEventUrl(reasonEvent.Id, accountName);
                        html.WriteLink(reasonEventUrl, " событие " + componentEventType.DisplayName);

                        if (reasonEvent.Message != null)
                        {
                            html.Write(": " + reasonEvent.Message);
                        }

                        var reasonEventProperties = storage.EventProperties.GetByEventId(reasonEvent.Id);
                        if (reasonEventProperties.Length > 0)
                        {
                            html.NewLine();
                            html.NewLine();
                            WriteEventPropertiesTable(html, reasonEventProperties, "Дополнительные свойства события компонента:");
                        }
                    }

                    // причина - статус дочернего компонента
                    if (reasonEvent.Category == EventCategory.ComponentExternalStatus)
                    {
                        var childComponent = storage.Components.GetOneById(reasonEvent.OwnerId);

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
                        var unitTest = storage.UnitTests.GetOneById(reasonEvent.OwnerId);

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

                        var reasonEventProperties = storage.EventProperties.GetByEventId(reasonEvent.Id);
                        if (reasonEventProperties.Length > 0)
                        {
                            html.NewLine();
                            html.NewLine();
                            WriteEventPropertiesTable(html, reasonEventProperties, "Дополнительные свойства результата проверки:");
                        }
                    }

                    // причина - значение метрики
                    if (reasonEvent.Category == EventCategory.MetricStatus)
                    {
                        var metric = storage.Metrics.GetOneById(reasonEvent.OwnerId);
                        var metricType = storage.MetricTypes.GetOneById(metric.MetricTypeId);

                        var firstValue = storage.MetricHistory.GetFirstByStatusEventId(reasonEvent.Id);

                        var firstValueText = firstValue != null ? firstValue.Value.ToString() : "null";

                        // Делаем строку:
                        // Причиной стало значение метрики CPU, % = 200

                        html.Write("Причиной стало ");
                        var title = "значение метрики " + metricType.DisplayName;

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

                var changeSettingsUrl = UrlHelper.GetSubscriptionEditUrl(component.Id, user.Id, accountName);
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

        protected void WriteEventPropertiesTable(HtmlRender html, EventPropertyForRead[] properties, string header)
        {
            if (properties.Length > 0)
            {
                html.Write(header);
                html.NewLine();

                html.BeginTable();
                foreach (var param in properties)
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
