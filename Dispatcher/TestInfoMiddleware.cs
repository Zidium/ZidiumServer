using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Zidium.Api;
using Zidium.Core;
using Zidium.Core.Caching;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;

namespace Zidium.Dispatcher
{
    public class TestInfoMiddleware
    {
        public TestInfoMiddleware(RequestDelegate next)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string testInfo;
            try
            {
                testInfo = GetTestInfo();
            }
            catch (Exception exception)
            {
                testInfo = exception.ToString();
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

            await httpContext.Response.WriteAsync(testInfo);
        }

        protected string GetTestInfo()
        {
            var result = SelfTest.Validate();
            return result.Log;
        }

        private static IComponentControl ComponentControl { get; set; }

        private static ComponentSelfTest SelfTest { get; set; }

        public static void Init(IComponentControl componentControl)
        {
            if (componentControl == null)
            {
                throw new ArgumentNullException("componentControl");
            }
            ComponentControl = componentControl;

            // запустим тест самопроверки
            SelfTest = new ComponentSelfTest(ComponentControl);
            SelfTest.AddUnitTest("CheckComponentControl", CheckComponentControl);
            SelfTest.AddUnitTest("CheckDbContext", CheckDbContext);
            SelfTest.AddUnitTest("AllCaches.Events.LastSaveException", () => CheckException(AllCaches.Events.LastSaveException));
            SelfTest.AddUnitTest("AllCaches.StatusDatas.LastSaveException", () => CheckException(AllCaches.StatusDatas.LastSaveException));
            SelfTest.AddUnitTest("AllCaches.UnitTests.LastSaveException", () => CheckException(AllCaches.UnitTests.LastSaveException));

            SelfTest.StartTimer(TimeSpan.FromMinutes(1));
        }

        private static void CheckComponentControl()
        {
            if (ComponentControl.IsFake())
            {
                throw new Exception("Component control is fake");
            }
        }

        private static void CheckDbContext()
        {
            var defaultStorageFactory = DependencyInjection.GetServicePersistent<IDefaultStorageFactory>();
            defaultStorageFactory.GetStorage().Check();
        }

        private static void CheckException(ExceptionTempInfo exception)
        {
            if (exception == null || exception.Exception == null)
            {
                return;
            }
            var duration = DateTime.Now - exception.Date;
            if (duration < TimeSpan.FromMinutes(10))
            {
                throw exception.Exception;
            }
        }


    }
}
