using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web;
using NLog;
using Zidium.Agent.AgentTasks.UnitTests.HttpRequests;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common.Helpers;

namespace Zidium.Agent.AgentTasks.HttpRequests
{
    public class HttpRequestsProcessor : UnitTestProcessorBase
    {
        public HttpRequestsProcessor(ILogger logger, CancellationToken cancellationToken)
            : base(logger, cancellationToken)
        {
        }

        protected override Guid GetUnitTestTypeId()
        {
            return SystemUnitTestTypes.HttpUnitTestType.Id;
        }

        protected override UnitTestExecutionInfo GetResult(Guid accountId,
            AccountDbContext accountDbContext,
            UnitTest unitTest,
            ILogger logger,
            CancellationToken token)
        {
            var rules = unitTest.HttpRequestUnitTest.Rules
                .Where(x => x.IsDeleted == false)
                .OrderBy(x => x.SortNumber)
                .ToList();

            logger.Debug("Найдено правил " + rules.Count);
            if (rules.Count > 0)
            {
                // Проверим наличие баннера, если это нужно
                if (unitTest.HttpRequestUnitTest.LastBannerCheck == null ||
                    unitTest.HttpRequestUnitTest.LastBannerCheck.Value <= DateTime.Now.AddHours(-24))
                {
                    var bannerResultData = CheckForBanner(accountId, accountDbContext, unitTest, logger);

                    if (bannerResultData != null)
                        return bannerResultData;
                }

                var resultData = new UnitTestExecutionInfo();
                resultData.Properties = new List<ExtentionPropertyDto>();

                foreach (var rule in rules)
                {
                    token.ThrowIfCancellationRequested();

                    var ruleResult = ProcessRule(rule, logger);

                    if (ruleResult.ErrorCode == HttpRequestErrorCode.UnknownError)
                    {
                        // если произошла неизвестная ошибка, то не обновляем статус проверки
                        logger.Debug("Выход из-за неизвестной ошибки");
                        return null;
                    }

                    // обновим правило
                    rule.LastRunErrorCode = ruleResult.ErrorCode;
                    rule.LastRunTime = ruleResult.StartDate;
                    rule.LastRunDurationMs = (int)(ruleResult.EndDate - ruleResult.StartDate).TotalMilliseconds;
                    rule.LastRunErrorMessage = ruleResult.ErrorMessage;

                    // сохраним информацию об ошибке
                    if (ruleResult.ErrorCode != HttpRequestErrorCode.Success)
                    {
                        resultData.Properties.AddValue("Rule", rule.DisplayName);
                        resultData.Properties.AddValue("ErrorMessage", rule.LastRunErrorMessage);
                        resultData.Properties.AddValue("ErrorCode", rule.LastRunErrorCode.ToString());
                        resultData.Properties.AddValue("RequestMethod", rule.Method.ToString());
                        resultData.Properties.AddValue("TimeoutMs", (rule.TimeoutSeconds ?? 0) * 1000);

                        var duration = (int)((ruleResult.EndDate - ruleResult.StartDate).TotalMilliseconds);
                        resultData.Properties.AddValue("ExecutionTimeMs", duration);

                        if (ruleResult.Request != null)
                        {
                            resultData.Properties.AddValue("RequestUrl", ruleResult.Request.RequestUri.AbsoluteUri);
                        }

                        if (ruleResult.ResponseHeaders != null)
                        {
                            resultData.Properties.AddValue("ResponseHeaders", ruleResult.ResponseHeaders);
                        }
                        if (ruleResult.ResponseHtml != null)
                        {
                            resultData.Properties.AddValue("ResponseHtml", ruleResult.ResponseHtml);
                        }

                        resultData.Result = UnitTestResult.Alarm;
                        var errorMessage = rule.DisplayName + ". " + rule.LastRunErrorMessage;
                        resultData.Message = errorMessage;
                        resultData.IsNetworkProblem = ruleResult.IsNetworkProblem;

                        accountDbContext.SaveChanges();

                        return resultData;
                    }
                }

                resultData.Result = UnitTestResult.Success;
                resultData.Message = "Успешно";

                // сохраним изменения статусов правил
                accountDbContext.SaveChanges();

                return resultData;
            }

            return new UnitTestExecutionInfo()
            {
                Result = UnitTestResult.Unknown,
                Message = "Укажите хотя бы один запрос (правило) для проверки"
            };
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

        protected HttpRequestErrorCode CheckResponse(
            HttpRequestResultInfo httpResult,
            int timeOutSeconds,
            HttpWebResponse response,
            ILogger logger)
        {
            var rule = httpResult.Rule;
            httpResult.ErrorMessage = null;

            // headers
            httpResult.ResponseHeaders = response.Headers.ToString();

            // html body
            var html = string.Empty;
            var responseStream = response.GetResponseStream();
            if (responseStream != null)
            {
                var streamReader = new StreamReader(responseStream, Encoding.UTF8); //todo почему всегда Encoding.UTF8?
                html = streamReader.ReadToEnd();
            }

            httpResult.ResponseHtml = html;

            // проверка responseCode
            httpResult.HttpStatusCode = response.StatusCode;
            var responseCode = (int)response.StatusCode;
            var validResponseCode = rule.ResponseCode ?? 200;
            if (responseCode != validResponseCode)
            {
                // неверный код ответа
                var message = string.Format(
                    "Неверный ResponseCode (получено {0}, должно быть {1})",
                    responseCode,
                    validResponseCode);

                httpResult.ErrorMessage = message;
                logger.Info(message);
                logger.Debug("ReponseHtml: " + html);
                return HttpRequestErrorCode.InvalidResponseCode;
            }

            // проверка ContentLength
            if (rule.MaxResponseSize > 0 && response.ContentLength > rule.MaxResponseSize)
            {
                // слишком большой ответ
                var message = string.Format(
                    "Превышен ContentLength (получено {0}, должно быть максимум {1})",
                    response.ContentLength,
                    rule.MaxResponseSize);

                httpResult.ErrorMessage = message;
                logger.Info(message);
                return HttpRequestErrorCode.TooLargeResponse;
            }

            // проверка timeOut
            var endTime = DateTime.Now;
            var duration = endTime - httpResult.StartDate;
            if (duration.TotalSeconds > timeOutSeconds)
            {
                // превышение тайматута
                var message = string.Format(
                    "Превышен таймаут выполнения (получилось {0} секунд, должно быть максимум {1} секунд)",
                    duration,
                    timeOutSeconds);

                httpResult.ErrorMessage = message;
                logger.Info(message);
                return HttpRequestErrorCode.Timeout;
            }

            if (string.IsNullOrEmpty(rule.SuccessHtml) == false && html.Contains(rule.SuccessHtml) == false)
            {
                // не найден обязательный фрагмент html
                var message = "Не найден обязательный фрагмент html";
                if (message.Length > 90)
                {
                    message = message.Substring(90) + "...";
                }
                httpResult.ErrorMessage = message;
                logger.Info(message);
                return HttpRequestErrorCode.SuccessHtmlNotFound;
            }
            if (string.IsNullOrEmpty(rule.ErrorHtml) == false && html.Contains(rule.ErrorHtml))
            {
                // найден недопустимый фрагмент html
                var message = "Найден недопустимый фрагмент html: " + rule.ErrorHtml;
                if (message.Length > 90)
                {
                    message = message.Substring(90) + "...";
                }
                httpResult.ErrorMessage = message;
                logger.Info(message);
                return HttpRequestErrorCode.ErrorHtmlFound;
            }

            // ура, все ОК
            return HttpRequestErrorCode.Success;
        }

        protected HttpRequestResultInfo ProcessRule(HttpRequestUnitTestRule rule, ILogger logger)
        {
            var result = new HttpRequestResultInfo
            {
                ErrorCode = HttpRequestErrorCode.Success,
                Rule = rule,
                StartDate = DateTime.Now
            };
            try
            {
                if (rule == null)
                {
                    throw new ArgumentNullException("rule");
                }

                logger.Debug("Правило: " + rule.DisplayName);
                if (string.IsNullOrWhiteSpace(rule.Url))
                {
                    result.ErrorCode = HttpRequestErrorCode.UrlFormatError;
                    result.ErrorMessage = "Укажите URL";
                    return result;
                }
                Uri uri = null;
                try
                {
                    uri = new Uri(rule.Url);
                }
                catch (UriFormatException)
                {
                    result.ErrorCode = HttpRequestErrorCode.UrlFormatError;
                    result.ErrorMessage = "Неверный формат URL";
                    return result;
                }
                if (new[] { "http", "https" }.Contains(uri.Scheme.ToLowerInvariant()) == false)
                {
                    result.ErrorCode = HttpRequestErrorCode.UrlFormatError;
                    result.ErrorMessage = "Неверная схема URI. Используйте http или https";
                    return result;
                }
                if (NetworkHelper.IsDomainHasIp(uri.Host) == false)
                {
                    result.ErrorCode = HttpRequestErrorCode.UnknownDomain;
                    result.ErrorMessage = "Не удалось получить IP-адрес";
                    return result;
                }

                var formDatas = rule.GetWebFormsDatas();
                var requestHeaders = rule.GetRequestHeaders();
                var requestCookies = rule.GetRequestCookies();

                // установим таймаут
                var timeOutSeconds = rule.TimeoutSeconds ?? 60;
                if (timeOutSeconds > 60)
                {
                    timeOutSeconds = 60;
                }

                // обновим URL
                if (rule.Method == HttpRequestMethod.Get && formDatas.Count > 0)
                {
                    var uriBuilder = new UriBuilder(uri);
                    var httpValueCollection = HttpUtility.ParseQueryString(uriBuilder.Uri.Query);
                    foreach (var webFormData in formDatas)
                    {
                        httpValueCollection.Add(webFormData.Key, webFormData.Value);
                    }
                    uriBuilder.Query = httpValueCollection.ToString();
                    uri = uriBuilder.Uri;
                }

                // создадим запрос
                var request = (HttpWebRequest)WebRequest.Create(uri);
                logger.Debug("Uri: " + request.RequestUri.AbsoluteUri);
                request.MaximumAutomaticRedirections = 4;
                request.MaximumResponseHeadersLength = 20;
                request.Timeout = timeOutSeconds * 1000;
                request.ReadWriteTimeout = timeOutSeconds * 1000;
                result.Request = request;

                // добавим заголовки HTTP
                foreach (var header in requestHeaders)
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

                // Обязательно заполним UserAgent
                // Пустого UserAgent быть не должно
                // Многие сервера выдают ошибку из-за этого
                if (string.IsNullOrEmpty(request.UserAgent))
                    request.UserAgent = "Zidium";

                // добавим куки
                foreach (var cookie in requestCookies)
                {
                    request.CookieContainer.Add(new Cookie(cookie.Key, cookie.Value));
                }

                // добавим данные веб-формы
                if (rule.Method == HttpRequestMethod.Post)
                {
                    request.Method = "POST";

                    string postData;
                    if (!string.IsNullOrEmpty(rule.Body))
                    {
                        request.ContentType = request.ContentType ?? "text/utf-8";
                        postData = rule.Body;
                    }
                    else
                    {
                        request.ContentType = "application/x-www-form-urlencoded";
                        var httpValueCollection = HttpUtility.ParseQueryString(string.Empty);
                        foreach (var formData in formDatas)
                        {
                            httpValueCollection.Add(formData.Key, formData.Value);
                        }

                        postData = httpValueCollection.ToString();
                    }

                    var data = Encoding.UTF8.GetBytes(postData);
                    request.ContentLength = postData.Length;

                    using (var dataStream = request.GetRequestStream())
                    {
                        dataStream.Write(data, 0, data.Length);
                        dataStream.Close();
                    }
                }

                // получаем ответ
                try
                {
                    var response = (HttpWebResponse)request.GetResponse();
                    result.ErrorCode = CheckResponse(result, timeOutSeconds, response, logger);
                }
                catch (SocketException exception)
                {
                    result.ErrorMessage = exception.Message;
                    result.ErrorCode = HttpRequestErrorCode.TcpError;
                    result.IsNetworkProblem = true;
                    logger.Error(exception);
                }
                catch (IOException exception)
                {
                    result.ErrorMessage = exception.Message;
                    result.ErrorCode = HttpRequestErrorCode.TcpError;
                    result.IsNetworkProblem = true;
                    logger.Error(exception);
                }
                catch (WebException exception)
                {
                    result.ErrorMessage = exception.Message;
                    if (exception.Response != null)
                    {
                        var httpResponse = (HttpWebResponse)exception.Response;
                        if (result.ResponseHeaders == null)
                        {
                            result.ResponseHeaders = GetResponseHeaders(httpResponse);
                        }
                        if (result.ResponseHtml == null)
                        {
                            result.ResponseHtml = GetResponseHtml(httpResponse);
                        }
                        if (result.HttpStatusCode == null)
                        {
                            result.HttpStatusCode = httpResponse.StatusCode;
                        }
                    }
                    if (exception.Status == WebExceptionStatus.Timeout)
                    {
                        result.ErrorCode = HttpRequestErrorCode.Timeout;
                        result.IsNetworkProblem = true;
                    }
                    else if (exception.Status == WebExceptionStatus.ProtocolError)
                    {
                        result.ErrorCode = HttpRequestErrorCode.InvalidResponseCode;
                    }
                    else
                    {
                        result.ErrorCode = HttpRequestErrorCode.TcpError;
                        result.IsNetworkProblem = true;
                        logger.Error(exception);
                    }
                }
            }
            catch (WebException exception)
            {
                result.ErrorCode = HttpRequestErrorCode.TcpError;
                result.ErrorMessage = exception.Message;
                result.IsNetworkProblem = true;
                logger.Error(exception);
            }
            catch (Exception exception)
            {

                exception.Data.Add("UnitTestId", rule.HttpRequestUnitTest.UnitTestId);
                exception.Data.Add("RuleId", rule.Id);
                exception.Data.Add("RuleName", rule.DisplayName);
                exception.Data.Add("UnitTestName", rule.HttpRequestUnitTest.UnitTest.DisplayName);
                logger.Error(exception);

                result.ErrorCode = HttpRequestErrorCode.UnknownError;
                result.ErrorMessage = exception.Message;
            }
            finally
            {
                result.EndDate = DateTime.Now;
                logger.Info(result.Rule.Url + " => " + result.ErrorCode);
            }
            return result;
        }

        protected UnitTestExecutionInfo CheckForBanner(
            Guid accountId,
            AccountDbContext accountDbContext,
            UnitTest unitTest,
            ILogger logger)
        {
            logger.Debug("Поиск баннера Zidium для проверки " + unitTest.DisplayName);
            UnitTestExecutionInfo result = null;

            // Из всех правил получим главные уникальные страницы сайтов
            var urls = unitTest.HttpRequestUnitTest.Rules.Select(t =>
            {
                var uri = new Uri(t.Url);
                return uri.Scheme + @"://" + uri.Authority;
            }).Distinct().ToArray();

            // Проверим, что на каждой главной странице есть баннер
            var hasBanner = true;
            string noBannerUrl = null;
            foreach (var url in urls)
            {
                logger.Debug("Поиск баннера Zidium на странице " + url);
                try
                {
                    var hasPageBanner = BannerHelper.FindBanner(url);
                    logger.Debug("Результат: " + hasPageBanner + ", страница: " + url);

                    if (!hasPageBanner)
                    {
                        noBannerUrl = url;
                        hasBanner = false;
                        break;
                    }
                }
                catch (Exception exception)
                {
                    logger.Debug("Ошибка проверки баннера " + url + ": " + exception.Message);
                    noBannerUrl = url;
                    hasBanner = false;
                    break;
                }
            }

            // Отправляем результат диспетчеру
            // Признак баннера будет обновлён, если лимит превышен, вернётся false
            var dispatcher = GetDispatcherClient();
            var response = dispatcher.SendHttpUnitTestBanner(accountId, unitTest.Id, hasBanner);
            if (!response.Success)
            {
                logger.Error("Не удалось проверить лимит http-проверок без баннера: " + response.ErrorMessage);
                return null;
            }

            // Если лимит превышен, то проверка уже отключена диспетчером, осталось поменять результат
            if (response.Data.CanProcessUnitTest == false)
            {
                result = new UnitTestExecutionInfo()
                {
                    Result = UnitTestResult.Alarm,
                    Message = "Проверка отключена, не найден баннер Zidium на странице " + noBannerUrl
                };
                logger.Warn(result.Message);
            }
            else
            {
                if (!hasBanner)
                    logger.Debug("Баннер Zidium не найден, но проверка включена, так как не превышен лимит бесплатных проверок");
                else
                    logger.Info("Баннер Zidium подтверждён для проверки " + unitTest.DisplayName);
            }
            accountDbContext.SaveChanges();
            return result;
        }
    }
}
