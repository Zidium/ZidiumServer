using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using ApiAdapter;
using Zidium.Api;
using Zidium.Api.Dto;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;

namespace Zidium.ApiHttpService
{
    public class ApiHandler : WebHandlerBase
    {
        private string GetExceptionTempString(ExceptionTempInfo exception)
        {
            if (exception == null)
            {
                return "";
            }
            return exception.Date + " " + exception.Exception;
        }

        public static ExceptionTempInfo RunException { get; set; }

        public static DateTime RunTime { get; set; }

        public static ApiToDispatcherAdapter CreateAdapter(string ip, string accountName)
        {
            return new ApiToDispatcherAdapter(Dispatcher, ip, accountName);
        }

        private static readonly IDispatcherService Dispatcher;

        public static IComponentControl ComponentControl { get; private set; }
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
            SelfTest.AddUnitTest("GetServerTime", GetServerTimeTest);
            SelfTest.AddUnitTest("CheckComponentControl", CheckComponentControl);
            SelfTest.StartTimer(TimeSpan.FromMinutes(1));
        }

        private static void GetServerTimeTest()
        {
            var adapter = CreateAdapter("127.0.0.1", "system");
            adapter.GetServerTime().Check();
        }

        private static void CheckComponentControl()
        {
            if (ComponentControl.IsFake())
            {
                throw new Exception("Component control is fake");
            }
        }

        protected override string GetDebugInfo()
        {
            var output = new StringBuilder();
            if (ComponentControl == null)
            {
                output.AppendLine("ComponentControl is NULL");
            }
            else
            {
                output.AppendLine("ComponentControl.IsFake()=" + ComponentControl.IsFake());
            }
            output.AppendLine("DispatcherService.Wrapper.RunTime = " + RunTime);
            output.AppendLine("DispatcherService.Wrapper.RunException = " + GetExceptionTempString(RunException));

            // проверка связи с диспетчером
            var adapter = CreateAdapter("127.0.0.1", "system");
            var response = adapter.GetServerTime();
            output.AppendLine("GetServerTime = " + (response.Success ? response.Data.Date.ToString() : response.ErrorMessage));

            return output.ToString();
        }

        protected override string GetTestInfo()
        {
            var result = SelfTest.Validate();
            return result.Log;
        }

        public override void ProcessRequest(HttpContext httpContext, out string action)
        {
            var timer = new Stopwatch();
            timer.Start();

            base.ProcessRequest(httpContext, out action);

            timer.Stop();

            // Считаем статистику только по корректным названиям action
            if (!string.IsNullOrEmpty(action))
            {
                AddRequestDuration((long)timer.Elapsed.TotalMilliseconds);
                UpdateCounters();
            }
        }

        #region Метрики

        protected static Stopwatch CounterTimer = new Stopwatch();

        protected static Int64 RequestsCount = 0;

        protected static Int64 RequestsDuration = 0;

        protected static object CounterLockObject = new object();

        protected static void UpdateCounters()
        {
            lock (CounterLockObject)
            {
                RequestsCount++;
                if (CounterTimer.Elapsed.TotalMinutes >= 1)
                {
                    var avgRequestDuration = RequestsDuration / (RequestsCount > 0 ? RequestsCount : 1);
                    var selfPercent = (RequestsDuration - InvokeDuration) * 100 / (RequestsDuration > 0 ? RequestsDuration : 1);
                    var avgInvokeDuration = InvokeDuration / (RequestsCount > 0 ? RequestsCount : 1);

                    if (ComponentControl != null)
                    {
                        var actualInterval = TimeSpan.FromHours(1);
                        ComponentControl.SendMetric("Запросов в минуту", RequestsCount, actualInterval);
                        ComponentControl.SendMetric("Мс на запрос", avgRequestDuration, actualInterval);
                        ComponentControl.SendMetric("Процент собственных действий", selfPercent, actualInterval);
                        ComponentControl.SendMetric("Мс на обработку", avgInvokeDuration, actualInterval);
                    }

                    RequestsCount = 0;
                    RequestsDuration = 0;
                    Interlocked.Exchange(ref InvokeDuration, 0);

                    CounterTimer.Restart();
                }
            }
        }

        protected static void AddRequestDuration(long ms)
        {
            lock (CounterLockObject)
            {
                RequestsDuration += ms;
            }
        }

        #endregion

        protected override object GetRealHandler(string ip, HttpContext httpContext)
        {
            var host = httpContext.Request.Url.Host;
            var index = host.IndexOf('.');
            var accountName = index >= 0 ? host.Substring(0, index) : FixedAccountName;
            return CreateAdapter(ip, accountName);
        }

        protected string FixedAccountName
        {
            get
            {
                if (_fixedAccountName == null)
                {
                    _fixedAccountName = DependencyInjection.GetServicePersistent<IApiHttpServiceConfiguration>().FixedAccountName;
                    if (_fixedAccountName == null)
                        throw new Exception("Не заполнена настройка FixedAccountName в конфигурационном файле");
                }

                return _fixedAccountName;
            }
        }

        private string _fixedAccountName;

        protected override MethodInfo GetMethodInfo(string action)
        {
            return typeof(IDtoService).GetMethod(action);
        }

        public static string GetVersion()
        {
            return typeof(ApiHandler).Assembly.GetName().Version.ToString();
        }

        protected override int GetErrorCode()
        {
            return ResponseCode.WebServiceError;
        }

        static ApiHandler()
        {
            Dispatcher = DispatcherHelper.GetDispatcherService();
            CounterTimer.Start();
        }
    }
}