using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Net.Http.Headers;
using Zidium.Api;

namespace Zidium.Dispatcher
{
    public class HttpServiceEventPreparer : IEventPreparer
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Prepare(SendEventBase eventObj)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return;
            }

            // Если событие не связано с запросом, то выход
            HttpRequest request;
            try
            {
                request = httpContext.Request;
            }
            catch
            {
                return;
            }

            if (request == null)
                return;

            var ip = httpContext.Connection.RemoteIpAddress?.ToString();
            eventObj.Properties.Set("IP", ip);

            var headersDictionary = httpContext.Request.Headers;
            eventObj.Properties.Set("UserAgent", headersDictionary[HeaderNames.UserAgent].ToString());
            eventObj.Properties.Set("Url", request.GetDisplayUrl());

            var body = GetBody(httpContext);
            if (!string.IsNullOrEmpty(body))
                eventObj.Properties.Set("Body", body);
        }

        public static string GetBody(HttpContext httpContext)
        {
            try
            {
                var stream = httpContext.Request.Body;
                using (var reader = new StreamReader(stream))
                {
                    var text = reader.ReadToEnd();
                    return text;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
