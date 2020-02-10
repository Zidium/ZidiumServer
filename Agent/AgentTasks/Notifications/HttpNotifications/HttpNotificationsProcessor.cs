using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using NLog;
using Zidium.Api.Dto;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;

namespace Zidium.Agent.AgentTasks.Notifications
{
    public class HttpNotificationsProcessor : NotificationSenderBase
    {
        public HttpNotificationsProcessor(ILogger logger, CancellationToken cancellationToken) 
            : base(logger, cancellationToken) { }

        protected override SubscriptionChannel[] Channels { get; } = new[] { SubscriptionChannel.Http };

        public static string GetNotificationJson(Component component, Event eventObj)
        {
            var jsonObject = new
            {
                Component = new
                {
                    Id = component.Id,
                    SystemName = component.SystemName,
                    DisplayName = component.DisplayName,
                    Properties = component.Properties.Select(t => new
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
                    Properties = eventObj.Properties.Select(t => new
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

        protected override void Send(ILogger logger,
            Notification notification,
            AccountDbContext accountDbContext,
            string accountName, Guid accountId)
        {
            var httpNotification = notification.NotificationHttp;
            var eventObj = notification.Event;
            
            if (eventObj == null)
            {
                throw new Exception("Event is null");
            }
            
            if (httpNotification.Json == null)
            {
                var componentRepository = accountDbContext.GetComponentRepository();
                var component = componentRepository.GetById(notification.Event.OwnerId);
                httpNotification.Json = GetNotificationJson(component, eventObj);
                accountDbContext.SaveChanges();
            }

            try
            {

                var httpWebRequest = (HttpWebRequest) WebRequest.Create(notification.Address);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Timeout = 10*1000; // Считаем, что получатель обязан принять уведомление за 10 секунд

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(httpNotification.Json);
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
