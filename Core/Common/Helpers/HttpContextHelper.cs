using System;
using System.IO;
using System.Web;
using Zidium.Api;

namespace Zidium.Core.Common.Helpers
{
    public static class HttpContextHelper
    {
        public static string GetClientIp(HttpContext context)
        {
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (!string.IsNullOrEmpty(ipAddress))
            {
                return ipAddress;
            }
            ipAddress = context.Request.ServerVariables["REMOTE_ADDR"];
            if (!string.IsNullOrEmpty(ipAddress))
            {
                return ipAddress;
            }
            ipAddress = context.Request.UserHostAddress;
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
                var stream = httpContext.Request.InputStream;
                stream.Position = 0;
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

        public static string GetBody(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                return null;
            }
            try
            {
                var stream = httpContext.Request.InputStream;
                stream.Position = 0;
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

        public static void SetProperties(ExtentionPropertyCollection properties)
        {
            var httpContext = HttpContext.Current;
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
            properties.Set("UserAgent", request.UserAgent);
            properties.Set("Url", request.Url.AbsoluteUri);

            // UrlReferrer
            if (request.UrlReferrer != null)
            {
                properties.Set("UrlReferrer", request.UrlReferrer.AbsoluteUri);
            }

            // Body
            var body = GetBody(httpContext);
            if (!string.IsNullOrEmpty(body))
                properties.Set("Body", body);
        }
    }
}
