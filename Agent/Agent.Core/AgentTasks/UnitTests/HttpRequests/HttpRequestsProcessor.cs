using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;
using Zidium.Agent.AgentTasks.UnitTests.HttpRequests;
using Zidium.Api.Dto;
using Zidium.Core.AccountDb;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks.HttpRequests
{
    public class HttpRequestsProcessor : UnitTestProcessorBase
    {
        public HttpRequestsProcessor(ILogger logger, CancellationToken cancellationToken, ITimeService timeService)
            : base(logger, cancellationToken, timeService)
        {
        }

        protected override Guid GetUnitTestTypeId()
        {
            return SystemUnitTestType.HttpUnitTestType.Id;
        }

        protected override UnitTestExecutionInfo GetResult(
            IStorage storage,
            UnitTestForRead unitTest)
        {
            var rules = storage.HttpRequestUnitTestRules.GetByUnitTestId(unitTest.Id)
                .OrderBy(x => x.SortNumber)
                .ToArray();

            Logger.LogDebug("Найдено правил " + rules.Length);

            if (rules.Length > 0)
            {
                var httpRequestUnitTest = storage.HttpRequestUnitTests.GetOneByUnitTestId(unitTest.Id);

                var resultData = new SendUnitTestResultRequestDataDto
                {
                    Properties = new List<ExtentionPropertyDto>()
                };

                foreach (var rule in rules)
                {
                    CancellationToken.ThrowIfCancellationRequested();

                    var ruleResult = ProcessRule(rule, storage, unitTest);

                    if (ruleResult.ErrorCode == HttpRequestErrorCode.UnknownError)
                    {
                        // если произошла неизвестная ошибка, то не обновляем статус проверки
                        Logger.LogDebug("Выход из-за неизвестной ошибки");

                        return new UnitTestExecutionInfo()
                        {
                            IsNetworkProblem = true
                        };
                    }

                    // обновим правило
                    var ruleForUpdate = rule.GetForUpdate();
                    ruleForUpdate.LastRunErrorCode.Set(ruleResult.ErrorCode);
                    ruleForUpdate.LastRunTime.Set(ruleResult.StartDate);
                    ruleForUpdate.LastRunDurationMs.Set((int)(ruleResult.EndDate - ruleResult.StartDate).TotalMilliseconds);
                    ruleForUpdate.LastRunErrorMessage.Set(ruleResult.ErrorMessage);
                    storage.HttpRequestUnitTestRules.Update(ruleForUpdate);

                    // сохраним информацию об ошибке
                    if (ruleResult.ErrorCode != HttpRequestErrorCode.Success)
                    {
                        resultData.Properties.AddValue("Rule", rule.DisplayName);
                        resultData.Properties.AddValue("ErrorMessage", ruleResult.ErrorMessage);
                        resultData.Properties.AddValue("ErrorCode", ruleResult.ErrorCode.ToString());
                        resultData.Properties.AddValue("RequestMethod", rule.Method.ToString());
                        resultData.Properties.AddValue("TimeoutMs", (rule.TimeoutSeconds ?? 0) * 1000);

                        var duration = (int)(ruleResult.EndDate - ruleResult.StartDate).TotalMilliseconds;
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
                        var errorMessage = rule.DisplayName + ". " + ruleResult.ErrorMessage;
                        resultData.Message = errorMessage;

                        return new UnitTestExecutionInfo()
                        {
                            ResultRequest = resultData,
                            IsNetworkProblem = ruleResult.IsNetworkProblem
                        };
                    }
                }

                resultData.Result = UnitTestResult.Success;
                resultData.Message = "Успешно";

                return new UnitTestExecutionInfo()
                {
                    ResultRequest = resultData
                };
            }

            return new UnitTestExecutionInfo()
            {
                ResultRequest = new SendUnitTestResultRequestDataDto()
                {
                    Result = UnitTestResult.Unknown,
                    Message = "Укажите хотя бы один запрос (правило) для проверки"
                }
            };
        }

        protected HttpRequestResultInfo ProcessRule(
            HttpRequestUnitTestRuleForRead rule,
            IStorage storage,
            UnitTestForRead unitTest)
        {
            var processor = new HttpTestProcessor(Logger);
            var inputData = new HttpTestInputData()
            {
                UnitTestId = rule.HttpRequestUnitTestId,
                ErrorHtml = rule.ErrorHtml,
                SuccessHtml = rule.SuccessHtml,
                MaxResponseSize = rule.MaxResponseSize,
                Method = rule.Method,
                Url = rule.Url,
                TimeoutSeconds = rule.TimeoutSeconds,
                ResponseCode = rule.ResponseCode
            };

            if (!string.IsNullOrEmpty(rule.Body))
            {
                inputData.Body = Encoding.UTF8.GetBytes(rule.Body);
            }

            var ruleDatas = storage.HttpRequestUnitTestRuleDatas.GetByRuleId(rule.Id);
            if (ruleDatas.Length > 0)
            {
                // данные формы
                var formDatas = ruleDatas.GetWebFormsDatas();
                if (formDatas.Length > 0)
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
                var cookies = ruleDatas.GetRequestCookies();
                if (cookies.Length > 0)
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
                var headers = ruleDatas.GetRequestHeaders();
                if (headers.Length > 0)
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
                StartDate = TimeService.Now(),
                EndDate = TimeService.Now()
            };

            try
            {
                var outputData = processor.Process(inputData);
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
                exception.Data.Add("UnitTestId", unitTest.Id);
                exception.Data.Add("RuleId", rule.Id);
                exception.Data.Add("RuleName", rule.DisplayName);
                exception.Data.Add("UnitTestName", unitTest.DisplayName);
                Logger.LogError(exception, exception.Message);

                result.ErrorCode = HttpRequestErrorCode.UnknownError;
                result.ErrorMessage = exception.Message;
            }
            return result;
        }

    }
}
