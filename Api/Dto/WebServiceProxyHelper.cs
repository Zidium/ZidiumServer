using System;
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

            try
            {
                var actionUri = new Uri(handlerUrl.TrimEnd('/') + "/" + action);

                using (var httpClient = new HttpClient())
                {
                    var content = requestObj != null ? serializer.GetString(requestObj) : string.Empty;
                    var httpContext = new StringContent(content);
                    httpContext.Headers.ContentType.CharSet = "utf-8";
                    httpContext.Headers.ContentType.MediaType = "application/json";
                    httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET API " + Tools.ApiVersion);

                    var response = httpClient.PostAsync(actionUri, httpContext).Result;

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
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

                        //(Client.Instance as Client).InternalLog.Trace("Request: " + content);
                        //(Client.Instance as Client).InternalLog.Trace("Response: " + Encoding.UTF8.GetString(responseBytes));

                        return responseData;
                    }
                    catch (Exception exception)
                    {
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
                var response = new TResponse
                {
                    Code = ResponseCode.ClientError,
                    ErrorMessage = exception.Message
                };
                return response;
            }
        }
    }
}
