using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Zidium.Api;

namespace Zidium.UserAccount
{
    public static class HttpContextHelper
    {
        public static string GetClientIp(HttpContext context)
        {
            string ipAddress = context.GetServerVariable("HTTP_X_FORWARDED_FOR");
            if (!string.IsNullOrEmpty(ipAddress))
            {
                return ipAddress;
            }
            ipAddress = context.GetServerVariable("REMOTE_ADDR");
            if (!string.IsNullOrEmpty(ipAddress))
            {
                return ipAddress;
            }
            ipAddress = context.Connection.RemoteIpAddress?.ToString();
            return ipAddress;
        }

        public static string GetBody(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                return null;
            }
            try
            {
                var stream = httpContext.Request.Body;
                using (var reader = new StreamReader(stream))
                {
                    var text = reader.ReadToEndAsync().Result;
                    return text;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void SetProperties(HttpContext httpContext, ExtentionPropertyCollection properties)
        {
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

            var ip = GetClientIp(httpContext);
            properties.Set("IP", ip);
            properties.Set("UserAgent", request.Headers["User-Agent"].ToString());
            properties.Set("Url", request.GetDisplayUrl());

            // UrlReferrer
            if (request.Headers["Referer"].Count > 0)
            {
                properties.Set("UrlReferrer", request.Headers["Referer"].ToString());
            }

            // Body
            var body = GetBody(httpContext);
            if (!string.IsNullOrEmpty(body))
                properties.Set("Body", body);
        }
    }
}
