using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Zidium.Api;
using Zidium.Api.Dto;
using Zidium.Core;

namespace Zidium.Dispatcher
{
    public abstract class WebHandlerMiddlewareBase
    {
        protected WebHandlerMiddlewareBase(ILogger logger)
        {
            Logger = logger;
        }

        protected readonly ILogger Logger;

        protected class ProcessRequestData
        {
            public HttpContext HttpContext { get; set; }

            public string UserAgent { get; set; }

            public string Ip { get; set; }

            public string Url { get; set; }

            public string ContentType { get; set; }

            public string Action { get; set; }

            public string Body { get; set; }
        }

        public async Task<string> InvokeInternal(HttpContext httpContext)
        {
            string action = null;

            if (_isStopping)
            {
                httpContext.Response.StatusCode = 503;
                return action;
            }

            ResponseDto response = null;

            var processData = new ProcessRequestData()
            {
                HttpContext = httpContext,
                Url = httpContext.Request.GetDisplayUrl()
            };

            try
            {
                // получим IP
                processData.Ip = httpContext.Connection.RemoteIpAddress?.ToString();

                // получим action
                action = GetAction(httpContext);
                processData.Action = action;

                if (string.IsNullOrEmpty(action))
                {
                    response = GetErrorResponse(ResponseCode.UnknownAction, "Не указано действие");
                }
                else
                {
                    // получим метод
                    var method = GetMethodInfo(action);
                    if (method == null)
                    {
                        response = GetErrorResponse(ResponseCode.UnknownAction, "Неизвестное действие: " + action);
                        action = null;
                    }
                    else
                    {
                        // получим тело
                        var body = await GetRequestBody(httpContext);

                        if (body.Length < (httpContext.Request.ContentLength ?? 0))
                        {
                            throw new IncompleteRequestException(body.Length, httpContext.Request.ContentLength ?? 0);
                        }

                        processData.Body = Encoding.UTF8.GetString(body);

                        // получим serializer
                        processData.ContentType = httpContext.Request.ContentType;

                        // получим пакет запроса
                        var requestPackage = GetRequestPackage(method, processData.Body);

                        // получим обработчик
                        var handler = GetRealHandler();

                        // получим response
                        var timer = new Stopwatch();
                        timer.Start();

                        response = InvokeMethod(handler, method, requestPackage);

                        timer.Stop();
                        AddInvokeDuration((long)timer.Elapsed.TotalMilliseconds);
                    }
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception exception)
            {
                response = HandleException(exception, processData);
            }
            finally
            {
                if (response == null)
                {
                    response = GetErrorResponse(ResponseCode.ServerError, "Неизвестная ошибка сервера");
                }
            }

            var responseHeaders = httpContext.Response.GetTypedHeaders();
            responseHeaders.CacheControl = new CacheControlHeaderValue()
            {
                NoCache = true,
                NoStore = true,
                MustRevalidate = true
            };

            var mediaType = new MediaTypeHeaderValue("application/json");
            mediaType.Encoding = Encoding.UTF8;
            httpContext.Response.ContentType = mediaType.ToString();

            var responseString = JsonSerializer.GetString(response);
            var responseBytes = Encoding.UTF8.GetBytes(responseString);
            httpContext.Response.ContentLength = responseBytes.Length;

            await httpContext.Response.BodyWriter.AsStream().WriteAsync(responseBytes);

            return action;
        }

        public string GetAction(HttpContext context)
        {
            var path = context.Request.GetDisplayUrl();
            var index = path.LastIndexOf("/", StringComparison.OrdinalIgnoreCase);
            if (index > -1)
            {
                var action = path.Substring(index + 1);
                return action;
            }
            return null;
        }

        protected ResponseDto GetErrorResponse(int responseCode, string message)
        {
            return new ResponseDto()
            {
                Code = responseCode,
                ErrorMessage = message
            };
        }

        protected abstract MethodInfo GetMethodInfo(string action);

        protected abstract object GetRealHandler();

        public async Task<byte[]> GetRequestBody(HttpContext context)
        {
            using (var stream = new MemoryStream())
            {
                await context.Request.Body.CopyToAsync(stream);
                return stream.ToArray();
            }
        }

        private static readonly JsonSerializer JsonSerializer = new JsonSerializer();

        protected object GetRequestPackage(MethodInfo method, string body)
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
            var package = JsonSerializer.GetObject(packageType, body);
            return package;
        }

        public ResponseDto InvokeMethod(object handler, MethodInfo method, object requestObj)
        {
            var methodParameters = method.GetParameters();
            if (methodParameters.Length == 0)
            {
                var responseObject = method.Invoke(handler, null);
                return (ResponseDto)responseObject;
            }
            else
            {
                if (requestObj == null)
                {
                    return GetErrorResponse(ResponseCode.RequestParseError, "Не заполнен запрос");
                }
                var responseObject = method.Invoke(handler, new[] { requestObj });
                return responseObject as ResponseDto;
            }
        }

        protected abstract int GetErrorCode();

        protected virtual ResponseDto HandleException(Exception exception, ProcessRequestData data)
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
            Logger.LogError(exception, exception.Message);

            var errorCode = GetErrorCode();

            return new ResponseDto()
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
            typeof(ArgumentException) // Бывает при неверном значении enum в json
        };

        private static bool _isStopping;

        public static void Stop()
        {
            _isStopping = true;
        }

        #region Метрики

        protected static long InvokeDuration;

        protected static void AddInvokeDuration(long ms)
        {
            Interlocked.Add(ref InvokeDuration, ms);
        }

        #endregion

    }
}
