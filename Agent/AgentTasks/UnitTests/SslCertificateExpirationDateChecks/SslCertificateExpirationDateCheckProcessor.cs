using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using NLog;
using Zidium.Common;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks
{
    public class SslCertificateExpirationDateCheckProcessor : UnitTestProcessorBase
    {
        public SslCertificateExpirationDateCheckProcessor(ILogger logger, CancellationToken cancellationToken, ITimeService timeService)
            : base(logger, cancellationToken, timeService)
        {
        }

        public static DateTime GetPaymentDate(Uri uri)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.AllowAutoRedirect = false;

            using (var response = (HttpWebResponse) request.GetResponse())
            {
                response.Close();
            }

            //retrieve the ssl cert and assign it to an X509Certificate object
            var cert = request.ServicePoint.Certificate;

            if (cert == null)
                throw new ArgumentNullException("cert");

            //convert the X509Certificate to an X509Certificate2 object by passing it into the constructor
            var cert2 = new X509Certificate2(cert);
            var value = cert2.GetExpirationDateString();
            var date = ParseHelper.TryParseDateTime(value);
            if (date == null)
            {
                throw new UserFriendlyException("Не удалось получить дату из строки: " + value);
            }
            return date.Value;
        }

        protected override Guid GetUnitTestTypeId()
        {
            return SystemUnitTestType.SslTestType.Id;
        }

        protected override UnitTestExecutionInfo GetResult(Guid accountId,
            IStorage storage,
            UnitTestForRead unitTest,
            ILogger logger,
            string accountName,
            CancellationToken token)
        {
            var rule = storage.UnitTestSslCertificateExpirationDateRules.GetOneOrNullByUnitTestId(unitTest.Id);
            var uri = new Uri(rule.Url);

            var ruleForUpdate = rule.GetForUpdate();
            if (uri.Scheme != "https")
            {
                ruleForUpdate.LastRunErrorCode.Set(SslCertificateExpirationDateErrorCode.UnknownError);
                storage.UnitTestSslCertificateExpirationDateRules.Update(ruleForUpdate);

                return new UnitTestExecutionInfo()
                {
                    ResultRequest = new SendUnitTestResultRequestData()
                    {
                        Message = "Url должен начинаться с https://",
                        Result = UnitTestResult.Alarm
                    }
                };
            }

            DateTime date;
            try
            {
                date = GetPaymentDate(uri);
            }
            catch (WebException exception)
            {
                ruleForUpdate.LastRunErrorCode.Set(SslCertificateExpirationDateErrorCode.UnknownError);
                storage.UnitTestSslCertificateExpirationDateRules.Update(ruleForUpdate);

                return new UnitTestExecutionInfo()
                {
                    ResultRequest = new SendUnitTestResultRequestData()
                    {
                        Result = UnitTestResult.Alarm,
                        Message = exception.Message,
                    },
                    IsNetworkProblem = true
                };
            }

            var days = (date.Date - DateTime.Now.Date).TotalDays;
            ruleForUpdate.LastRunErrorCode.Set(SslCertificateExpirationDateErrorCode.Success);
            storage.UnitTestSslCertificateExpirationDateRules.Update(ruleForUpdate);

            var result = new UnitTestExecutionInfo()
            {
                ResultRequest = new SendUnitTestResultRequestData()
                {
                    Message = string.Format(
                        "Осталось {0} дней до окончания срока действия сертификата. Срок действия {1}",
                        days,
                        date.ToString("dd.MM.yyyy"))
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
    }
}
