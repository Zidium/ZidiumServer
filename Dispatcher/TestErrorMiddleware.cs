using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Zidium.Dispatcher
{
    public class TestErrorMiddleware
    {
        public TestErrorMiddleware(RequestDelegate next, ILogger<TestErrorMiddleware> logger)
        {
            _logger = logger;
        }

        private readonly ILogger _logger;

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                throw new Exception("TEST EXCEPTION");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
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
