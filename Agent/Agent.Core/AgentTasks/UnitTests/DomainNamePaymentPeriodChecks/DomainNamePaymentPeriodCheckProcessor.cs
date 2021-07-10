using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web;
using NLog;
using Zidium.Api.Dto;
using Zidium.Common;
using Zidium.Core.AccountsDb;
using Zidium.Core.Common;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks
{
    public class DomainNamePaymentPeriodCheckProcessor : UnitTestProcessorBase
    {
        public DomainNamePaymentPeriodCheckProcessor(ILogger logger, CancellationToken cancellationToken, ITimeService timeService)
            : base(logger, cancellationToken, timeService)
        {
        }

        private static int GetIndex(string text, int startIndex, string searchText)
        {
            int index = text.IndexOf(searchText, startIndex, StringComparison.InvariantCultureIgnoreCase);
            if (index < 1)
            {
                throw new UserFriendlyException("Не удалось найти фрагмент html: " + searchText);
            }
            return index;
        }

        /// <summary>
        /// Проверяет что строка начинается с года, например, 2017-01-30
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static bool IsYearBeginFormat(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }
            if (text.Length > 5 && text[4] == '-')
            {
                for (int i = 0; i < 4; i++)
                {
                    if (char.IsNumber(text[i]) == false)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public static DateTime ParseAnyFormatDate(string dateText)
        {
            if (dateText == null)
            {
                throw new ArgumentNullException("dateText");
            }
            dateText = dateText.Trim();

            // если есть буквы
            if (dateText.ToCharArray().Any(x => char.IsLetter(x)))
            {
                dateText = dateText.Replace('/', '-');
                if (IsYearBeginFormat(dateText))
                {
                    dateText = dateText.Substring(0, 10);
                    return DateTime.ParseExact(dateText, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
                return DateTime.ParseExact(dateText, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
            }
            return ParseHelper.ParseDateTime(dateText);
        }

        [Obsolete]
        public static DomainNamePaymentPeriodCheckResult GetPaymentDateFromNicRu(string host)
        {
            var client = new HttpClient();

            // Конкретно для nic.ru название кодируется в windows-1251
            var url = "https://www.nic.ru/whois/?searchWord=" + HttpUtility.UrlEncode(host, Encoding.GetEncoding("windows-1251"));
            var uri = new Uri(url);

            string html;
            try
            {
                html = client.GetStringAsync(uri).Result;
            }
            catch (Exception exception)
            {
                return new DomainNamePaymentPeriodCheckResult()
                {
                    Code = DomainNamePaymentPeriodErrorCode.Unavailable,
                    Date = null,
                    ErrorMessage = Api.Tools.GetExceptionFullMessage(exception)
                };
            }

            if (html.IndexOf("Доменное имя свободно!", StringComparison.CurrentCultureIgnoreCase) > 0)
            {
                if (html.IndexOf("paid-till:", StringComparison.CurrentCultureIgnoreCase) == -1)
                {
                    return new DomainNamePaymentPeriodCheckResult()
                    {
                        Code = DomainNamePaymentPeriodErrorCode.FreeDomain,
                        ErrorMessage = "Доменное имя свободно"
                    };
                }
            }

            if (html.IndexOf("Справка по поддоменам домена", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                return new DomainNamePaymentPeriodCheckResult()
                {
                    Code = DomainNamePaymentPeriodErrorCode.SubDomain,
                    ErrorMessage = "Справка по поддоменам не предоставляется"
                };
            }

            // по данным WHOIS.NIC.RU:
            var whoisNicRu = "<h3>по данным WHOIS.NIC.RU:</h3>";
            if (html.IndexOf(whoisNicRu, StringComparison.InvariantCultureIgnoreCase) > 0)
            {
                int index = GetIndex(html, 0, whoisNicRu);
                if (html.Substring(index).Contains("paid-till:&nbsp; &nbsp;"))
                {
                    index = GetIndex(html, index, "paid-till:&nbsp; &nbsp;");
                    int start = index + "paid-till:&nbsp; &nbsp;".Length + 1;
                    string dateString = html.Substring(start, 10);
                    DateTime date = ParseHelper.ParseDateTime(dateString);
                    return new DomainNamePaymentPeriodCheckResult()
                    {
                        Code = DomainNamePaymentPeriodErrorCode.Success,
                        Date = date
                    };
                }
                if (html.Substring(index).Contains("Registrar Registration Expiration Date:"))
                {
                    index = GetIndex(html, index, "Registrar Registration Expiration Date:");
                    int start = index + "Registrar Registration Expiration Date:".Length + 1;
                    string dateString = html.Substring(start, "2019-01-03T21:00:00Z".Length);
                    DateTime date = ParseHelper.ParseDateTime(dateString);
                    return new DomainNamePaymentPeriodCheckResult()
                    {
                        Code = DomainNamePaymentPeriodErrorCode.Success,
                        Date = date
                    };
                }
            }

            // по данным WHOIS.TCINET.RU:
            var whoisTcinet = "по данным WHOIS.TCINET.RU:";
            if (html.IndexOf(whoisTcinet, StringComparison.InvariantCultureIgnoreCase) > 0)
            {
                int index = GetIndex(html, 0, whoisTcinet);
                if (html.Substring(index).Contains("paid-till:&nbsp; &nbsp;&nbsp;"))
                {
                    index = GetIndex(html, index, "paid-till:&nbsp; &nbsp;&nbsp;");
                    int start = index + "paid-till:&nbsp; &nbsp;&nbsp;".Length + 1;
                    string dateString = html.Substring(start, 10);
                    DateTime date = ParseHelper.ParseDateTime(dateString);
                    return new DomainNamePaymentPeriodCheckResult()
                    {
                        Code = DomainNamePaymentPeriodErrorCode.Success,
                        Date = date
                    };
                }
                if (html.Substring(index).Contains("Registrar Registration Expiration Date:"))
                {
                    index = GetIndex(html, index, "Registrar Registration Expiration Date:");
                    int start = index + "Registrar Registration Expiration Date:".Length + 1;
                    string dateString = html.Substring(start, "2019-01-03T21:00:00Z".Length);
                    DateTime date = ParseHelper.ParseDateTime(dateString);
                    return new DomainNamePaymentPeriodCheckResult()
                    {
                        Code = DomainNamePaymentPeriodErrorCode.Success,
                        Date = date
                    };
                }
            }

            // Сервис временно недоступен
            if (html.IndexOf("Ошибка: сервис временно недоступен (503)", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                return new DomainNamePaymentPeriodCheckResult()
                {
                    Code = DomainNamePaymentPeriodErrorCode.Unavailable,
                    Date = null,
                    ErrorMessage = "Сервис временно недоступен"
                };
            }

            // еще по каким то данным... попробуем угадать
            var parser = new TextParser(html);
            parser.StringComparison = StringComparison.InvariantCultureIgnoreCase;

            var index1 = parser.First("<div class=\"b-whois-info\">").To;
            var index2 = parser.First("</div>").From;
            var divHtml = parser.GetBetween(index1, index2);
            parser = new TextParser(divHtml.Substring);

            var searchStrings = new[]
            {
                "Registry Expiry Date:",
                "Expiration Date:",
                "expires:"
            };
            foreach (var searchString in searchStrings)
            {
                var dateFragment = parser.FirstOrNull(searchString);
                if (dateFragment != null)
                {
                    index1 = dateFragment.To;
                    index2 = parser.First("<br>").From;
                    var dateText = parser.GetBetween(index1, index2);
                    var dateString = dateText.Substring.Replace("&nbsp;", "").Trim();
                    DateTime date = ParseAnyFormatDate(dateString);

                    return new DomainNamePaymentPeriodCheckResult()
                    {
                        Code = DomainNamePaymentPeriodErrorCode.Success,
                        Date = date
                    };
                }
            }

            return new DomainNamePaymentPeriodCheckResult()
            {
                Code = DomainNamePaymentPeriodErrorCode.UnknownError,
                ErrorMessage = "Не удалось найти фрагмент поля срока оплаты, host = " + host,
                Date = null,
                Html = html
            };
        }

        protected override Guid GetUnitTestTypeId()
        {
            return SystemUnitTestType.DomainNameTestType.Id;
        }

        protected override UnitTestExecutionInfo GetResult(
            IStorage storage,
            UnitTestForRead unitTest)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }

            var rule = storage.DomainNamePaymentPeriodRules.GetOneOrNullByUnitTestId(unitTest.Id);
            if (rule == null)
                return null; // Правило ещё не создалось, выход

            if (string.IsNullOrWhiteSpace(rule.Domain))
            {
                throw new UserFriendlyException("Не указан домен для проверки");
            }

            var resultInfo = GetPaymentDate(rule.Domain);

            // Сервис недоступен
            if (resultInfo.Code == DomainNamePaymentPeriodErrorCode.Unavailable)
            {
                return new UnitTestExecutionInfo()
                {
                    ResultRequest = new SendUnitTestResultRequestDataDto()
                    {
                        Message = resultInfo.ErrorMessage,
                        Result = UnitTestResult.Unknown,
                    },
                    IsNetworkProblem = true
                };
            }

            if (resultInfo.Code == DomainNamePaymentPeriodErrorCode.UnknownError)
            {
                var logEvent = new LogEventInfo()
                {
                    Level = NLog.LogLevel.Warn,
                    Message = "Произошла неизвестная ошибка"
                };
                logEvent.Properties["Html"] = resultInfo.Html;
                Logger.Log(logEvent);

                LastError = resultInfo.ErrorMessage;
            }

            var ruleForUpdate = rule.GetForUpdate();
            if (resultInfo.Date == null)
            {
                ruleForUpdate.LastRunErrorCode.Set(resultInfo.Code);
                storage.DomainNamePaymentPeriodRules.Update(ruleForUpdate);

                return new UnitTestExecutionInfo()
                {
                    ResultRequest = new SendUnitTestResultRequestDataDto()
                    {
                        Result = UnitTestResult.Alarm,
                        Message = resultInfo.ErrorMessage
                    }
                };
            }

            var days = (int)((resultInfo.Date.Value - DateTime.Now.Date).TotalDays);
            ruleForUpdate.LastRunErrorCode.Set(DomainNamePaymentPeriodErrorCode.Success);
            storage.DomainNamePaymentPeriodRules.Update(ruleForUpdate);

            var result = new UnitTestExecutionInfo()
            {
                ResultRequest = new SendUnitTestResultRequestDataDto()
                {
                    Message = string.Format(
                        "Осталось {0} дней до окончания срока оплаты. Домен оплачен до {1}",
                        days,
                        resultInfo.Date.Value.ToString("dd.MM.yyyy"))
                }
            };
            if (days <= rule.AlarmDaysCount)
            {
                result.ResultRequest.Result = UnitTestResult.Alarm;
            }
            else if (days <= rule.WarningDaysCount)
            {
                result.ResultRequest.Result = UnitTestResult.Warning;
            }
            else
            {
                result.ResultRequest.Result = UnitTestResult.Success;
            }
            return result;
        }

        public static DomainNamePaymentPeriodCheckResult GetPaymentDate(string host)
        {
            List<string> info;

            try
            {
                Uri uri;
                try
                {
                    uri = new Uri(host);
                }
                catch
                {
                    uri = new Uri("http://" + host);
                }

                info = Whois.Lookup(uri.Host, WhoIsRecordType.Domain);
            }
            catch (Exception exception)
            {
                return new DomainNamePaymentPeriodCheckResult()
                {
                    Code = DomainNamePaymentPeriodErrorCode.Unavailable,
                    Date = null,
                    ErrorMessage = Api.Tools.GetExceptionFullMessage(exception)
                };
            }

            if (info.Any(t => t == "No entries found for the selected source(s)."
                              || t.StartsWith("No match for ")))
            {
                return new DomainNamePaymentPeriodCheckResult()
                {
                    Code = DomainNamePaymentPeriodErrorCode.FreeDomain,
                    ErrorMessage = "Доменное имя свободно"
                };
            }

            var data = info.Select(t =>
            {
                if (t.IndexOf(": ", StringComparison.Ordinal) == -1)
                    return null;

                var pair = t.Split(new[] { ": " }, StringSplitOptions.None);

                return new Pair
                {
                    Key = pair[0].Trim(),
                    Value = pair[1].Trim(),
                };
            })
                .Where(t => t != null)
                .Distinct(new PairComparer())
                .ToDictionary(a => a.Key, b => b.Value);

            var searchStrings = new[]
            {
                "Registry Expiry Date",
                "paid-till",
                "expires",
                "Registrar Registration Expiration Date"
            };

            foreach (var searchString in searchStrings)
            {
                if (data.TryGetValue(searchString, out var dateText))
                {
                    var date = ParseAnyFormatDate(dateText);

                    return new DomainNamePaymentPeriodCheckResult()
                    {
                        Code = DomainNamePaymentPeriodErrorCode.Success,
                        Date = date
                    };
                }
            }

            return new DomainNamePaymentPeriodCheckResult()
            {
                Code = DomainNamePaymentPeriodErrorCode.UnknownError,
                ErrorMessage = "Не удалось найти фрагмент поля срока оплаты, host = " + host,
                Date = null,
                Html = string.Join(Environment.NewLine, info)
            };
        }

        private class Pair
        {
            public string Key;
            public string Value;
        }

        private class PairComparer : IEqualityComparer<Pair>
        {
            public bool Equals(Pair x, Pair y)
            {
                return x?.Key == y?.Key;
            }

            public int GetHashCode(Pair obj)
            {
                return obj.Key.GetHashCode();
            }
        }
    }
}
