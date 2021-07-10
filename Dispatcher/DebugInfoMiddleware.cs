using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Zidium.Api;
using Zidium.Common;
using Zidium.Core.Api;
using Zidium.Core.Caching;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;

namespace Zidium.Dispatcher
{
    public class DebugInfoMiddleware
    {
        public DebugInfoMiddleware(RequestDelegate next)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string debugInfo;
            try
            {
                debugInfo = GetDebugInfo();
            }
            catch (Exception exception)
            {
                debugInfo = exception.ToString();
            }

            var responseHeaders = httpContext.Response.GetTypedHeaders();
            responseHeaders.CacheControl = new CacheControlHeaderValue()
            {
                NoCache = true,
                NoStore = true,
                MustRevalidate = true
            };

            var mediaType = new MediaTypeHeaderValue("text/plain");
            mediaType.Encoding = Encoding.UTF8;
            httpContext.Response.ContentType = mediaType.ToString();

            await httpContext.Response.WriteAsync(debugInfo);
        }

        protected string GetDebugInfo()
        {
            var output = new StringBuilder();

            output.AppendLine("Version " + VersionHelper.GetProductVersion());
            output.AppendLine("DispatcherWrapper.Uptime = " + DispatcherWrapper.UptimeTimer.Elapsed);
            output.AppendLine("------------");

            if (!ComponentControl.IsFake())
            {
                output.AppendLine("Api.EventManager.GetQueueSize(): " + DataSizeHelper.GetSizeText(ComponentControl.Client.EventManager.GetQueueSize()));
                output.AppendLine("Api.WebLogManager.GetQueueSize(): " + DataSizeHelper.GetSizeText(ComponentControl.Client.WebLogManager.GetQueueSize()));
            }
            output.AppendLine("------------");
            output.AppendLine("");

            foreach (var cache in AllCaches.All)
            {
                output.AppendLine("--- " + cache.GetType() + " ---");
                output.AppendLine("Count: " + cache.Count + " / " + cache.MaxCount);
                output.AppendLine("Size: " + DataSizeHelper.GetSizeText(cache.GetSize()));
                output.AppendLine("Changed: " + cache.GetChangedCount());
                output.AppendLine("LastSaveChangesDate: " + cache.GetLastSaveChangesDate());
                output.AppendLine("AddCacheCount: " + cache.AddCacheCount);
                output.AppendLine("AddDataBaseCount: " + cache.AddDataBaseCount);
                output.AppendLine("UpdateCacheCount: " + cache.UpdateCacheCount);
                output.AppendLine("UpdateDataBaseCount: " + cache.UpdateDataBaseCount);
                output.AppendLine("Generation: " + cache.Generation);
                output.AppendLine("LastSaveException: " + GetExceptionTempString(cache.LastSaveException));

                output.AppendLine("");
            }

            return output.ToString();
        }

        private static IComponentControl ComponentControl { get; set; }

        public static void Init(IComponentControl componentControl)
        {
            if (componentControl == null)
            {
                throw new ArgumentNullException("componentControl");
            }
            ComponentControl = componentControl;
        }

        private string GetExceptionTempString(ExceptionTempInfo exception)
        {
            if (exception == null)
            {
                return "";
            }
            return exception.Date + " " + exception.Exception.ToString();
        }
    }
}
