using System;
using System.IO;
using System.Net;

namespace Zidium.Api.Dto
{
    public class WebServiceProxyHelper
    {
        public TResponse ExecuteAction<TResponse>(
            string handlerUrl, 
            string action, 
            ISerializer serializer,
            object requestObj)
            where TResponse : Response, new()
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

            // может быть пустым, например, запрос времени
            //if (requestObj == null)
            //{
            //    throw new ArgumentNullException("requestObj");
            //}

            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;
            try
            {
                var actionUri = new Uri(handlerUrl.TrimEnd('/') + "/" + action);
                httpRequest = WebRequest.Create(actionUri) as HttpWebRequest;
                httpRequest.Method = "POST";
                if (serializer.Format == "xml" || serializer.Format == "json")
                {
                    httpRequest.ContentType = "application/" + serializer.Format;
                }
                else
                {
                    httpRequest.ContentType = "application/octet-stream";
                }

                httpRequest.UserAgent = ".NET API " + Tools.ApiVersion;
                httpRequest.KeepAlive = false; // todo надо подумать
                if (requestObj != null)
                {
                    byte[] requestBytes = serializer.GetBytes(requestObj);
                    httpRequest.ContentLength = requestBytes.Length;
                    using (var stream = httpRequest.GetRequestStream())
                    {
                        stream.Write(requestBytes, 0, requestBytes.Length);
                        stream.Close();
                    }
                }
                else // может быть пустым, например, запрос времени
                {
                    httpRequest.ContentLength = 0;
                    // Поток записи обязательно должен быть закрыт (во всяком случае, в CF)
                    using (var stream = httpRequest.GetRequestStream())
                    {
                        stream.Close();
                    }
                }

                httpResponse = httpRequest.GetResponse() as HttpWebResponse;
                if (httpResponse.StatusCode != HttpStatusCode.OK)
                {
                    return new TResponse()
                    {
                        Code = ResponseCode.InvalidHttpResponseCode,
                        ErrorMessage = "Получен неверный код HTTP-ответа " + ((int) httpResponse.StatusCode)
                    };
                }
                using (var responseStream = httpResponse.GetResponseStream())
                {
                    using (var binaryReader = new BinaryReader(responseStream))
                    {
                        var responseBytes = binaryReader.ReadBytes((int)httpResponse.ContentLength);
                        try
                        {
                            var responseData = (TResponse)serializer.GetObject(typeof(TResponse), responseBytes);
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
            finally
            {
                if (httpResponse != null)
                {
                    httpResponse.Close();
                }
            }
        }
    }
}
