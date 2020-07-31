using NLog;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks.UnitTests.HttpRequests
{
    public class HttpTestProcessor
    {
        private readonly ILogger _logger;

        public HttpTestProcessor()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public HttpTestProcessor(ILogger logger)
        {
            _logger = logger;
        }

        public HttpTestOutputData Process(HttpTestInputData inputData)
        {
            if (inputData == null)
            {
                throw new ArgumentNullException(nameof(inputData));
            }
            var outputData = new HttpTestOutputData
            {
                ErrorCode = HttpRequestErrorCode.Success,
                StartDate = DateTime.Now
            };
            try
            {
                // проверим, что url указан
                if (string.IsNullOrWhiteSpace(inputData.Url))
                {
                    outputData.ErrorCode = HttpRequestErrorCode.UrlFormatError;
                    outputData.ErrorMessage = "Укажите URL";
                    return outputData;
                }

                // проверим формат url
                Uri uri = null;
                try
                {
                    uri = new Uri(inputData.Url);
                }
                catch (UriFormatException)
                {
                    outputData.ErrorCode = HttpRequestErrorCode.UrlFormatError;
                    outputData.ErrorMessage = "Неверный формат URI";
                    return outputData;
                }

                // проверим схему
                if (new[] { "http", "https" }.Contains(uri.Scheme.ToLowerInvariant()) == false)
                {
                    outputData.ErrorCode = HttpRequestErrorCode.UrlFormatError;
                    outputData.ErrorMessage = "Неверная схема URI. Используйте http или https";
                    return outputData;
                }

                // проверим, что домен имеет IP
                if (NetworkHelper.IsDomainHasIp(uri.Host) == false)
                {
                    outputData.ErrorCode = HttpRequestErrorCode.UnknownDomain;
                    outputData.ErrorMessage = "Не удалось получить IP-адрес";
                    outputData.IsNetworkProblem = true; // были случаи когда иногда IP не определялся, хз почему
                    return outputData;
                }

                // установим таймаут
                var timeOutSeconds = inputData.TimeoutSeconds ?? 60;
                if (timeOutSeconds > 60)
                {
                    timeOutSeconds = 60;
                }

                // обновим URL для GET-запроса
                if (inputData.Method == HttpRequestMethod.Get
                    && inputData.FormParams != null
                    && inputData.FormParams.Count > 0)
                {
                    var uriBuilder = new UriBuilder(uri);
                    var httpValueCollection = HttpUtility.ParseQueryString(uriBuilder.Uri.Query);
                    foreach (var formParam in inputData.FormParams)
                    {
                        httpValueCollection.Add(formParam.Key, formParam.Value);
                    }
                    uriBuilder.Query = httpValueCollection.ToString();
                    uri = uriBuilder.Uri;
                }

                // поддержка протоколов SSL
                // todo пределать на настройки SSL для 1 запроса
                // https://stackoverflow.com/questions/3791629/set-the-securityprotocol-ssl3-or-tls-on-the-net-httpwebrequest-per-request/52064776#52064776
                // HttpClientHandler
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                       | SecurityProtocolType.Tls11
                                                       | SecurityProtocolType.Tls12
                                                       | SecurityProtocolType.Ssl3;

                // создадим запрос
                var request = (HttpWebRequest)WebRequest.Create(uri);
                if (_logger.IsDebugEnabled)
                {
                    _logger.Debug($"begin {inputData.Method} {request.RequestUri.AbsoluteUri}");
                }
                request.MaximumAutomaticRedirections = 4;
                request.MaximumResponseHeadersLength = 500;
                request.Timeout = timeOutSeconds * 1000;
                request.ReadWriteTimeout = timeOutSeconds * 1000;
                request.KeepAlive = false;

                // добавим заголовки HTTP
                if (inputData.Headers != null)
                {
                    foreach (var header in inputData.Headers)
                    {
                        if (string.Equals("Content-Type", header.Key, StringComparison.OrdinalIgnoreCase))
                        {
                            request.ContentType = header.Value;
                        }
                        else if (string.Equals("User-Agent", header.Key, StringComparison.OrdinalIgnoreCase))
                        {
                            request.UserAgent = header.Value;
                        }
                        else
                        {
                            request.Headers.Add(header.Key, header.Value);
                        }
                    }
                }


                // Обязательно заполним UserAgent
                // Пустого UserAgent быть не должно
                // Многие сервера выдают ошибку из-за этого
                if (string.IsNullOrEmpty(request.UserAgent))
                    request.UserAgent = "Zidium";

                // добавим куки
                if (inputData.Cookies != null)
                {
                    request.CookieContainer = new CookieContainer();
                    foreach (Cookie inputDataCookie in inputData.Cookies)
                    {
                        inputDataCookie.Domain = uri.Host;
                    }
                    request.CookieContainer.Add(inputData.Cookies);
                }

                // добавим данные веб-формы
                if (inputData.Method == HttpRequestMethod.Post)
                {
                    request.Method = "POST";

                    byte[] postData;
                    if (inputData.Body != null)
                    {
                        request.ContentType = request.ContentType ?? "text/utf-8";
                        postData = inputData.Body;
                    }
                    else
                    {
                        request.ContentType = "application/x-www-form-urlencoded";
                        var httpValueCollection = HttpUtility.ParseQueryString(string.Empty);
                        if (inputData.FormParams != null)
                        {
                            foreach (var formParam in inputData.FormParams)
                            {
                                httpValueCollection.Add(formParam.Key, formParam.Value);
                            }
                        }
                        string postBodyText = httpValueCollection.ToString();
                        postData = Encoding.UTF8.GetBytes(postBodyText);
                    }
                    request.ContentLength = postData.Length;

                    using (var dataStream = request.GetRequestStream())
                    {
                        dataStream.Write(postData, 0, postData.Length);
                        dataStream.Close();
                    }
                }

                // получаем ответ
                try
                {
                    var response = (HttpWebResponse)request.GetResponse();
                    outputData.ErrorCode = CheckResponse(inputData, outputData, timeOutSeconds, response);
                }
                catch (SocketException exception)
                {
                    exception.Data.Add("Account", inputData.AccountName);
                    exception.Data.Add("UnitTestId", inputData.UnitTestId);
                    outputData.Exceptions.Add(exception);
                    outputData.ErrorMessage = exception.Message;
                    outputData.ErrorCode = HttpRequestErrorCode.TcpError;
                    outputData.IsNetworkProblem = true;
                    _logger.Error(exception);
                }
                catch (IOException exception)
                {
                    exception.Data.Add("Account", inputData.AccountName);
                    exception.Data.Add("UnitTestId", inputData.UnitTestId);
                    outputData.Exceptions.Add(exception);
                    outputData.ErrorMessage = exception.Message;
                    outputData.ErrorCode = HttpRequestErrorCode.TcpError;
                    outputData.IsNetworkProblem = true;
                    _logger.Error(exception);
                }
                catch (WebException exception)
                {
                    exception.Data.Add("Account", inputData.AccountName);
                    exception.Data.Add("UnitTestId", inputData.UnitTestId);
                    outputData.Exceptions.Add(exception);
                    outputData.ErrorMessage = exception.Message;
                    if (exception.Response != null)
                    {
                        var httpResponse = (HttpWebResponse)exception.Response;
                        if (outputData.ResponseHeaders == null)
                        {
                            outputData.ResponseHeaders = GetResponseHeaders(httpResponse);
                        }
                        if (outputData.ResponseHtml == null)
                        {
                            outputData.ResponseHtml = GetResponseHtml(httpResponse);
                        }
                        if (outputData.HttpStatusCode == null)
                        {
                            outputData.HttpStatusCode = httpResponse.StatusCode;
                        }
                    }
                    if (exception.Status == WebExceptionStatus.Timeout)
                    {
                        outputData.ErrorCode = HttpRequestErrorCode.Timeout;
                        outputData.IsNetworkProblem = true;
                    }
                    else if (exception.Status == WebExceptionStatus.ProtocolError)
                    {
                        outputData.ErrorCode = HttpRequestErrorCode.InvalidResponseCode;
                    }
                    else
                    {
                        outputData.ErrorCode = HttpRequestErrorCode.TcpError;
                        outputData.IsNetworkProblem = true;
                        _logger.Error(exception);
                    }
                }
            }
            catch (WebException exception)
            {
                exception.Data.Add("Account", inputData.AccountName);
                exception.Data.Add("UnitTestId", inputData.UnitTestId);
                outputData.Exceptions.Add(exception);
                outputData.ErrorCode = HttpRequestErrorCode.TcpError;
                outputData.ErrorMessage = exception.Message;
                outputData.IsNetworkProblem = true;
                _logger.Error(exception);
            }
            catch (Exception exception)
            {
                exception.Data.Add("Account", inputData.AccountName);
                exception.Data.Add("UnitTestId", inputData.UnitTestId);
                outputData.Exceptions.Add(exception);
                _logger.Error(exception);
                outputData.ErrorCode = HttpRequestErrorCode.UnknownError;
                outputData.ErrorMessage = exception.Message;
            }
            finally
            {
                if (outputData.EndDate == null)
                {
                    outputData.EndDate = DateTime.Now;
                }
                _logger.Info(inputData.Url + " => " + outputData.ErrorCode);
            }
            return outputData;
        }

        protected HttpRequestErrorCode CheckResponse(
            HttpTestInputData inputData,
            HttpTestOutputData outputData,
            int timeOutSeconds,
            HttpWebResponse response)
        {
            // headers
            outputData.ResponseHeaders = response.Headers.ToString();

            // html body
            var html = string.Empty;
            using (var responseStream = response.GetResponseStream())
            {
                if (responseStream != null)
                {
                    var streamReader = new StreamReader(responseStream, Encoding.UTF8); //todo почему всегда Encoding.UTF8?
                    var task = streamReader.ReadToEndAsync();
                    var executedTask = Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(10))).Result;
                    if (executedTask == task)
                    {
                        html = task.Result;
                    }
                    else
                    {
                        responseStream.Close();
                    }
                }
            }

            outputData.ResponseHtml = html;

            // проверка responseCode
            outputData.HttpStatusCode = response.StatusCode;
            var responseCode = (int)response.StatusCode;
            var validResponseCode = inputData.ResponseCode ?? 200;
            if (responseCode != validResponseCode)
            {
                // неверный код ответа
                var message = string.Format(
                    "Неверный ResponseCode (получено {0}, должно быть {1})",
                    responseCode,
                    validResponseCode);

                outputData.ErrorMessage = message;
                _logger.Info(message);
                if (_logger.IsDebugEnabled)
                {
                    _logger.Debug("ReponseHtml: " + html);
                }
                return HttpRequestErrorCode.InvalidResponseCode;
            }

            // проверка ContentLength
            if (inputData.MaxResponseSize > 0 && response.ContentLength > inputData.MaxResponseSize)
            {
                // слишком большой ответ
                var message = string.Format(
                    "Превышен ContentLength (получено {0}, должно быть максимум {1})",
                    response.ContentLength,
                    inputData.MaxResponseSize);

                outputData.ErrorMessage = message;
                _logger.Info(message);
                return HttpRequestErrorCode.TooLargeResponse;
            }

            // проверка timeOut
            var duration = outputData.EndDate - outputData.StartDate;
            if (duration.TotalSeconds > timeOutSeconds)
            {
                // превышение тайматута
                var message = string.Format(
                    "Превышен таймаут выполнения (получилось {0} секунд, должно быть максимум {1} секунд)",
                    duration,
                    timeOutSeconds);

                outputData.ErrorMessage = message;
                _logger.Info(message);
                outputData.IsNetworkProblem = true;
                return HttpRequestErrorCode.Timeout;
            }

            // проверка обязательного фрагмента html
            if (string.IsNullOrEmpty(inputData.SuccessHtml) == false && html.Contains(inputData.SuccessHtml) == false)
            {
                var message = "Не найден обязательный фрагмент html";
                if (message.Length > 90)
                {
                    message = message.Substring(90) + "...";
                }
                outputData.ErrorMessage = message;
                _logger.Info(message);
                return HttpRequestErrorCode.SuccessHtmlNotFound;
            }

            // проверка недопустимого фрагмента html
            if (string.IsNullOrEmpty(inputData.ErrorHtml) == false && html.Contains(inputData.ErrorHtml))
            {
                // найден недопустимый фрагмент html
                var message = "Найден недопустимый фрагмент html: " + inputData.ErrorHtml;
                if (message.Length > 90)
                {
                    message = message.Substring(90) + "...";
                }
                outputData.ErrorMessage = message;
                _logger.Info(message);
                return HttpRequestErrorCode.ErrorHtmlFound;
            }

            // ура, все ОК
            return HttpRequestErrorCode.Success;
        }

        protected string GetResponseHeaders(HttpWebResponse response)
        {
            try
            {
                return response.Headers.ToString();
            }
            catch
            {
                return null;
            }
        }

        protected string GetResponseHtml(HttpWebResponse response)
        {
            try
            {
                var responseStream = response.GetResponseStream();

                if (responseStream == null)
                    return null;

                var streamReader = new StreamReader(responseStream, Encoding.UTF8); //todo почему всегда Encoding.UTF8?
                var html = streamReader.ReadToEnd();
                return html;
            }
            catch
            {
                return null;
            }
        }
    }
}
