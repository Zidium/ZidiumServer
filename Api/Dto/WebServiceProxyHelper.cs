using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Zidium.Api.Dto
{
    public static class WebServiceProxyHelper
    {
        public static TResponse ExecuteAction<TResponse>(
            string handlerUrl,
            string action,
            ISerializer serializer,
            object requestObj)
            where TResponse : ResponseDto, new()
        {
            if (handlerUrl == null)
            {
                throw new ArgumentNullException("handlerUrl");
            }
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }
            var success = false;
            var log = (Client.Instance as Client).InternalLog;
            var requestStopWatch = Stopwatch.StartNew();

            try
            {
                using (var httpClient = GetHttpClient(handlerUrl))
                {
                    var content = requestObj != null ? serializer.GetString(requestObj) : string.Empty;
                    if (log.IsTraceEnabled) log.Trace("Request " + action + ": " + content);

                    var httpContext = new StringContent(content);
                    httpContext.Headers.ContentType.CharSet = "utf-8";
                    httpContext.Headers.ContentType.MediaType = "application/json";

                    var response = httpClient.PostAsync(action, httpContext).Result;

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        if (log.IsTraceEnabled) log.Trace("Response " + action + " status code: " + response.StatusCode);

                        return new TResponse()
                        {
                            Code = ResponseCode.InvalidHttpResponseCode,
                            ErrorMessage = "Получен неверный код HTTP-ответа " + ((int)response.StatusCode)
                        };
                    }

                    var responseBytes = response.Content.ReadAsByteArrayAsync().Result;

                    try
                    {
                        var responseData = (TResponse)serializer.GetObject(typeof(TResponse), responseBytes);

                        if (log.IsTraceEnabled) log.Trace("Response " + action + ": " + Encoding.UTF8.GetString(responseBytes));

                        success = true;
                        return responseData;
                    }
                    catch (Exception exception)
                    {
                        log.Error("Request " + action + " failed: " + exception.Message, exception);

                        return new TResponse()
                        {
                            Code = ResponseCode.ResponseParseError,
                            ErrorMessage = "Ошибка парсинга ответа: " + exception.Message
                        };
                    }
                }

            }
            catch (Exception exception)
            {
                log.Error("Request " + action + " failed: " + exception.Message, exception);
                var response = new TResponse
                {
                    Code = ResponseCode.ClientError,
                    ErrorMessage = exception.Message
                };
                return response;
            }
            finally
            {
                if (log.IsTraceEnabled) log.Trace("Request " + action + " takes " + requestStopWatch.Elapsed);
                OnRequestCompleted?.Invoke(action, requestStopWatch.Elapsed, success);
            }
        }

        private static HttpClient GetHttpClient(string url)
        {
            if (ZidiumHttpClientFactory.HttpClientFactory != null)
                return ZidiumHttpClientFactory.HttpClientFactory.CreateClient("Zidium");

            var httpClient = new HttpClient();
            httpClient.Timeout = Client.Instance.Config.Access.Timeout;
            httpClient.BaseAddress = new Uri(url);
            httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET API " + Tools.ApiVersion);
            return httpClient;
        }

        public static event RequestCompletedHandler OnRequestCompleted;

        public delegate void RequestCompletedHandler(string action, TimeSpan elapsed, bool success);
    }
}