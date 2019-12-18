using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
            string accountName,
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

                    var ruleResult = ProcessRule(rule, logger, accountName);

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

        protected HttpRequestResultInfo ProcessRule(HttpRequestUnitTestRule rule, ILogger logger, string accountName)
        {
            var processor = new HttpTestProcessor(logger);
            var inputData = new HttpTestInputData()
            {
                AccountName = accountName,
                UnitTestId = rule.HttpRequestUnitTestId,
                ErrorHtml = rule.ErrorHtml,
                SuccessHtml = rule.SuccessHtml,
                MaxResponseSize = rule.MaxResponseSize,
                Method = rule.Method,
                Url = rule.Url,
                TimeoutSeconds = rule.TimeoutSeconds,
                ResponseCode = rule.ResponseCode
            };
            if (string.IsNullOrEmpty(rule.Body) == false)
            {
                inputData.Body = Encoding.UTF8.GetBytes(rule.Body);
            }
            if (rule.Datas != null)
            {
                // данные формы
                var formDatas = rule.GetWebFormsDatas();
                if (formDatas.Count > 0)
                {
                    var pars = new Dictionary<string, string>();
                    foreach (var formData in formDatas)
                    {
                        if (pars.ContainsKey(formData.Key) == false)
                        {
                            pars.Add(formData.Key, formData.Value);
                        }
                    }
                    inputData.FormParams = pars;
                }

                // куки
                var cookies = rule.GetRequestCookies();
                if (cookies.Count > 0)
                {
                    CookieCollection cookieCollection = new CookieCollection();
                    foreach (var cookie in cookies)
                    {
                        Cookie c = new Cookie(cookie.Key, cookie.Value);
                        cookieCollection.Add(c);
                    }
                    inputData.Cookies = cookieCollection;
                }

                // заголовки
                var headers = rule.GetRequestHeaders();
                if (headers.Count > 0)
                {
                    var pars = new Dictionary<string, string>();
                    foreach (var header in headers)
                    {
                        if (pars.ContainsKey(header.Key) == false)
                        {
                            pars.Add(header.Key, header.Value);
                        }
                    }
                    inputData.Headers = pars;
                }
            }

            var result = new HttpRequestResultInfo
            {
                ErrorCode = HttpRequestErrorCode.Success,
                Rule = rule,
                StartDate = DateTime.Now
            };
            try
            {
                HttpTestOutputData outputData = processor.Process(inputData);
                result.StartDate = outputData.StartDate;
                result.EndDate = outputData.EndDate;
                result.ErrorCode = outputData.ErrorCode;
                result.ErrorMessage = outputData.ErrorMessage;
                result.HttpStatusCode = outputData.HttpStatusCode;
                result.IsNetworkProblem = outputData.IsNetworkProblem;
                result.ResponseHeaders = outputData.ResponseHeaders;
                result.ResponseHtml = outputData.ResponseHtml;
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
                if (result.EndDate == null)
                {
                    result.EndDate = DateTime.Now;
                }
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
