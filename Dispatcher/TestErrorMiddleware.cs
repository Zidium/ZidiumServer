using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using NLog;

namespace Zidium.Dispatcher
{
    public class TestErrorMiddleware
    {
        public TestErrorMiddleware(RequestDelegate next)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                throw new Exception("TEST EXCEPTION");
            }
            catch (Exception exception)
            {
                LogManager.GetCurrentClassLogger().Error(exception);
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

            await httpContext.Response.WriteAsync("Test exception logged");
        }
    }
}
