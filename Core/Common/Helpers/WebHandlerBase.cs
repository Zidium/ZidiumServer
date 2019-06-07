using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using NLog;
using Zidium.Api;
using Zidium.Api.Dto;
using Zidium.Core.Common.Helpers;

namespace Zidium.Core.Common
{
    public abstract class WebHandlerBase : IHttpHandler
    {
        public class ProcessRequestData
        {
            public HttpContext HttpContext { get; set; }

            public string UserAgent { get; set; }

            public string Ip { get; set; }

            public string Url { get; set; }

            public string ContentType { get; set; }

            public string Action { get; set; }

            public string Body { get; set; }
        }

        public ISerializer GetSerializer(HttpContext context)
        {
            if (string.Equals(context.Request.ContentType, "application/xml", StringComparison.InvariantCultureIgnoreCase))
            {
                return XmlSerializer;
            }
            if (string.Equals(context.Request.ContentType, "application/octet-stream", StringComparison.InvariantCultureIgnoreCase))
            {
                return XmlDeflateSerializer;
            }
            if (string.Equals(context.Request.ContentType, "application/json", StringComparison.InvariantCultureIgnoreCase))
            {
                return JsonSerializer;
            }
            throw new WrongContentTypeException(context.Request.ContentType);
        }

        private static readonly XmlSerializer XmlSerializer = new XmlSerializer();
        private static readonly XmlDeflateSerializer XmlDeflateSerializer = new XmlDeflateSerializer();
        private static readonly JsonSerializer JsonSerializer = new JsonSerializer();

        public string GetAction(HttpContext context)
        {
            var path = context.Request.Url.AbsolutePath;
            var index = path.LastIndexOf("/", StringComparison.OrdinalIgnoreCase);
            if (index > -1)
            {
                var action = path.Substring(index + 1);
                return action;
            }
            return null;
        }

        public byte[] GetRequestBody(HttpContext context)
        {
            byte[] result;
            var stream = context.Request.InputStream;
            stream.Position = 0;
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                result = memoryStream.ToArray();
            }
            return result;
        }

        protected abstract MethodInfo GetMethodInfo(string action);

        protected abstract object GetRealHandler(string ip, HttpContext httpContext);

        protected object GetRequestPackage(MethodInfo method, ISerializer serializer, byte[] body)
        {
            if (method.GetParameters().Length == 0)
            {
                return null;
            }
            if (body.Length == 0)
            {
                return null;
            }
            var packageType = method.GetParameters()[0].ParameterType;
            var package = serializer.GetObject(packageType, body);
            return package;
        }

        public Response InvokeMethod(object handler, MethodInfo method, object requestObj)
        {
            var methodParameters = method.GetParameters();
            if (methodParameters.Length == 0)
            {
                var responseObject = method.Invoke(handler, null);
                return (Response)responseObject;
            }
            else
            {
                if (requestObj == null)
                {
                    return GetErrorResponse(ResponseCode.RequestParseError, "Не заполнен запрос");
                }
                var responseObject = method.Invoke(handler, new[] { requestObj });
                return responseObject as Response;
            }
        }

        protected Response GetErrorResponse(int responseCode, string message)
        {
            return new Response()
            {
                Code = responseCode,
                ErrorMessage = message
            };
        }

        protected abstract int GetErrorCode();

        protected virtual Response HandleException(Exception exception, ProcessRequestData data)
        {
            if (ExternalExceptions.Contains(exception.GetType()))
            {
                // TODO: Если ошибка наведённая (из-за некорректного запроса), то запишем её во встроенный компонент аккаунта
            }

            // Любая ошибка должна быть зарегистрирована
            // Ингнорирование настраивается с помощью перекрытия важности в ЛК
            exception.Data.Add("IP", data.Ip);
            exception.Data.Add("Url", data.Url);
            exception.Data.Add("UserAgent", data.UserAgent);
            exception.Data.Add("Action", data.Action);
            exception.Data.Add("Body", data.Body ?? "null");
            if (data.Body != null)
                exception.Data.Add("BodyLength", data.Body.Length);
            exception.Data.Add("ContentType", data.ContentType);
            exception.Data.Add("ContentLength", data.HttpContext.Request.ContentLength);
            Tools.HandleOutOfMemoryException(exception);
            LogManager.GetCurrentClassLogger().Error(exception);

            var errorCode = GetErrorCode();

            return new Response()
            {
                Code = errorCode,
                ErrorMessage = exception.Message
            };
        }

        protected static List<Type> ExternalExceptions = new List<Type>
        {
            typeof(IncompleteRequestException), // Из-за проблем с сетью данные могут передаться не до конца
            typeof(InvalidOperationException), // Клиент может прислать в запросе что угодно
            typeof(ArgumentNullException), // Какой-то обязательный параметр не заполнен в тексте json
            typeof(InvalidDataException), // Клиент может прислать в запросе что угодно
            typeof(WrongContentTypeException), //Неверный ContentType
            typeof(ArgumentException) // Бывает при неверном значении enum в json
        };

        public void ProcessRequest(HttpContext context)
        {
            if (_isStopping)
            {
                context.Response.StatusCode = 503;
                return;
            }

            ProcessRequest(context, out _);
        }

        protected abstract string GetDebugInfo();

        protected abstract string GetTestInfo();

        public virtual void ProcessRequest(HttpContext httpContext, out string action)
        {
            action = null;
            ISerializer serializer = null;
            Response response = null;

            var processData = new ProcessRequestData()
            {
                HttpContext = httpContext,
                Url = httpContext.Request.Url.ToString()
            };

            var isDebugRequest = false;

            try
            {
                // получим IP
                processData.Ip = HttpContextHelper.GetClientIp(httpContext);

                // получим action
                action = GetAction(httpContext);
                processData.Action = action;

                if (string.IsNullOrEmpty(action))
                {
                    response = GetErrorResponse(ResponseCode.UnknownAction, "Не указано действие");
                    return;
                }

                if (string.Equals("Debug", action, StringComparison.InvariantCultureIgnoreCase))
                {
                    isDebugRequest = true;
                    string debugInfo;
                    try
                    {
                        debugInfo = GetDebugInfo();
                    }
                    catch (Exception exception)
                    {
                        debugInfo = exception.ToString();
                    }

                    httpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    httpContext.Response.Cache.AppendCacheExtension("no-store, must-revalidate");
                    httpContext.Response.ContentType = "text/plain";
                    httpContext.Response.Charset = "utf-8";
                    httpContext.Response.Write(debugInfo);
                    return;
                }

                if (string.Equals("Test", action, StringComparison.InvariantCultureIgnoreCase))
                {
                    isDebugRequest = true;
                    string testInfo;
                    try
                    {
                        testInfo = GetTestInfo();
                    }
                    catch (Exception exception)
                    {
                        testInfo = exception.ToString();
                    }

                    httpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    httpContext.Response.Cache.AppendCacheExtension("no-store, must-revalidate");
                    httpContext.Response.ContentType = "text/plain";
                    httpContext.Response.Charset = "utf-8";
                    httpContext.Response.Write(testInfo);
                    return;
                }

                // получим метод
                var method = GetMethodInfo(action);
                if (method == null)
                {
                    response = GetErrorResponse(ResponseCode.UnknownAction, "Неизвестное действие: " + action);
                    action = null;
                    return;
                }

                // получим тело
                var body = GetRequestBody(httpContext);

                if (body.Length < httpContext.Request.ContentLength)
                {
                    throw new IncompleteRequestException(body.Length, httpContext.Request.ContentLength);
                }

                processData.Body = Encoding.UTF8.GetString(body);

                // получим serializer
                serializer = GetSerializer(httpContext);
                processData.ContentType = httpContext.Request.ContentType;

                // получим пакет запроса
                var requestPackage = GetRequestPackage(method, serializer, body);

                // получим обработчик
                var handler = GetRealHandler(processData.Ip, httpContext);

                // получим response
                var timer = new Stopwatch();
                timer.Start();

                response = InvokeMethod(handler, method, requestPackage);

                timer.Stop();
                AddInvokeDuration((long)timer.Elapsed.TotalMilliseconds);
            }
            catch (ThreadAbortException) { }
            catch (Exception exception)
            {
                response = HandleException(exception, processData);
            }
            finally
            {
                if (isDebugRequest == false)
                {
                    if (response == null)
                    {
                        response = GetErrorResponse(ResponseCode.ServerError, "Неизвестная ошибка сервера");
                    }
                    if (serializer == null)
                    {
                        serializer = new JsonSerializer();
                    }

                    httpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    httpContext.Response.Cache.AppendCacheExtension("no-store, must-revalidate");
                    httpContext.Response.ContentType = "application/" + serializer.Format;
                    httpContext.Response.Charset = "utf-8";

                    var responseBytes = serializer.GetBytes(response);
                    httpContext.Response.BinaryWrite(responseBytes);
                }
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }

        #region Метрики

        protected static long InvokeDuration;

        //protected static object CounterLockObject = new object();

        protected static void AddInvokeDuration(long ms)
        {
            Interlocked.Add(ref InvokeDuration, ms);
        }

        #endregion

        private static bool _isStopping;

        public static void Stop()
        {
            _isStopping = true;
        }

    }
}
