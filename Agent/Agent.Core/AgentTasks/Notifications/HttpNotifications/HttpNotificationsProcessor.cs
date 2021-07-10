using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using NLog;
using Zidium.Api.Dto;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks.Notifications
{
    public class HttpNotificationsProcessor : NotificationSenderBase
    {
        public HttpNotificationsProcessor(ILogger logger, CancellationToken cancellationToken)
            : base(logger, cancellationToken) { }

        protected override SubscriptionChannel[] Channels { get; } = new[] { SubscriptionChannel.Http };

        public static string GetNotificationJson(
            ComponentForRead component,
            ComponentPropertyForRead[] componentProperties,
            EventForRead eventObj,
            EventPropertyForRead[] eventProperties)
        {
            var jsonObject = new
            {
                Component = new
                {
                    Id = component.Id,
                    SystemName = component.SystemName,
                    DisplayName = component.DisplayName,
                    Properties = componentProperties.Select(t => new
                    {
                        Name = t.Name,
                        Value = t.Value,
                        DataType = t.DataType
                    }).ToArray()
                },
                Event = new
                {
                    Id = eventObj.Id,
                    Importance = eventObj.Importance.ToString(),
                    StartDate = eventObj.StartDate,
                    EndDate = eventObj.EndDate,
                    Properties = eventProperties.Select(t => new
                    {
                        Name = t.Name,
                        Value = t.Value,
                        DataType = t.DataType
                    }).ToArray()
                }
            };
            var json = new JsonSerializer().GetString(jsonObject);
            return json;
        }

        protected override void Send(
            NotificationForRead notification,
            IStorage storage,
            NotificationForUpdate notificationForUpdate)
        {
            var httpNotification = storage.NotificationsHttp.GetByNotificationId(notification.Id);
            var eventObj = storage.Events.GetOneById(notification.EventId);

            if (eventObj == null)
            {
                throw new Exception("Event is null");
            }

            var json = httpNotification.Json;
            if (json == null)
            {
                var component = storage.Components.GetOneById(eventObj.OwnerId);
                var componentProperties = storage.ComponentProperties.GetByComponentId(component.Id);
                var eventProperties = storage.EventProperties.GetByEventId(eventObj.Id);
                json = GetNotificationJson(component, componentProperties, eventObj, eventProperties);

                var httpNotificationForUpdate = httpNotification.GetForUpdate();
                httpNotificationForUpdate.Json.Set(json);
                storage.NotificationsHttp.Update(httpNotificationForUpdate);
            }

            try
            {

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(notification.Address);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 10 * 1000; // Считаем, что получатель обязан принять уведомление за 10 секунд

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Close();
                }
            }
            catch (Exception exception)
            {
                throw new NotificationNonImportantException(exception);
            }

            // по не будем проверять успешно обработаллсь HTTP-уведомление или нет
            // т.к. если неуспешно, то непонятно что делать
            // не будем же мы слать их до бесконечности

            //using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            //{
            //    int code = (int)httpResponse.StatusCode;
            //    if (code != 200)
            //        throw new HttpException(httpResponse.StatusDescription);
            //}
        }
    }
}
