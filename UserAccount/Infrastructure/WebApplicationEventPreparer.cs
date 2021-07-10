using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Zidium.Api;
using Zidium.Api.Dto;
using Zidium.Core.Common.Helpers;
using Zidium.UserAccount.Helpers;

namespace Zidium.UserAccount
{
    public class WebApplicationEventPreparer : IEventPreparer
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Prepare(SendEventBase eventObj)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            try
            {
                // Если UserAgent не указан, то не будем ничего делать,
                // это либо роботы, либо псевдо-хакеры
                // Игнорируем также проверки от самого Zidium
                if (httpContext != null)
                {
                    var userAgent = httpContext.Request.Headers[HeaderNames.UserAgent].ToString();
                    if (string.IsNullOrEmpty(userAgent) ||
                        string.Equals(userAgent, "Zidium", StringComparison.OrdinalIgnoreCase) ||
                        UserAgentHelper.IsBot(userAgent))
                    {
                        eventObj.Ignore = true;
                        return;
                    }
                }
            }
            catch
            {
                // Request доступен не для всех событий
            }

            HttpContextHelper.SetProperties(httpContext, eventObj.Properties);

            var user = UserHelper.CurrentUser(httpContext);
            if (user != null)
            {
                eventObj.Properties.Set("UserId", user.Id);
                eventObj.Properties.Set("UserLogin", user.Login);
                eventObj.Properties.Set("UserName", user.Name);
            }

            // Игнорирование некоторых ошибок
            if (eventObj.EventCategory == SendEventCategory.ApplicationError)
            {
                if (
                        // Несуществующий метод контроллера
                        eventObj.TypeSystemName.StartsWith("HttpException в Controller.HandleUnknownAction(String)", StringComparison.OrdinalIgnoreCase) ||
                        eventObj.TypeSystemName.StartsWith("HttpException at Controller.HandleUnknownAction(String)", StringComparison.OrdinalIgnoreCase) ||

                        // Несуществующий контроллер
                        eventObj.TypeSystemName.StartsWith("HttpException в DefaultControllerFactory.GetControllerInstance(RequestContext, Type)", StringComparison.OrdinalIgnoreCase) ||
                        eventObj.TypeSystemName.StartsWith("HttpException at DefaultControllerFactory.GetControllerInstance(RequestContext, Type)", StringComparison.OrdinalIgnoreCase) ||

                        // Неверный токен
                        eventObj.TypeSystemName.StartsWith("HttpAntiForgeryException", StringComparison.OrdinalIgnoreCase) ||

                        // Недопустимые символы в Url
                        eventObj.TypeSystemName.IndexOf("HttpRequest.ValidateInputIfRequiredByConfig()", StringComparison.OrdinalIgnoreCase) >= 0
                    )
                {
                    eventObj.Ignore = true;
                }
            }
        }
    }
}
