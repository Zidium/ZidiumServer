using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using NLog;
using Zidium.Core;
using Zidium.Core.AccountsDb;
using Zidium.Core.Api;
using Zidium.Core.Common;

namespace Zidium.Agent.AgentTasks
{
    public class SslCertificateExpirationDateCheckProcessor : UnitTestProcessorBase
    {
        public SslCertificateExpirationDateCheckProcessor(ILogger logger, CancellationToken cancellationToken)
            : base(logger, cancellationToken)
        {
        }

        public static DateTime GetPaymentDate(Uri uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            response.Close();

            //retrieve the ssl cert and assign it to an X509Certificate object
            X509Certificate cert = request.ServicePoint.Certificate;

            if (cert == null)
                throw new ArgumentNullException("cert");

            //convert the X509Certificate to an X509Certificate2 object by passing it into the constructor
            X509Certificate2 cert2 = new X509Certificate2(cert);
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
            return SystemUnitTestTypes.SslTestType.Id;
        }

        protected override SendUnitTestResultRequestData GetResult(
            Guid accountId, 
            AccountDbContext accountDbContext, 
            UnitTest unitTest, 
            ILogger logger,
            CancellationToken token)
        {
            var rule = unitTest.SslCertificateExpirationDateRule;
            var uri = new Uri(unitTest.SslCertificateExpirationDateRule.Url);

            if (uri.Scheme != "https")
            {
                rule.LastRunErrorCode = SslCertificateExpirationDateErrorCode.Success;
                return new SendUnitTestResultRequestData()
                {
                    Message = "Url должен начинаться с https://",
                    Result = UnitTestResult.Alarm
                };
            }

            DateTime date;
            try
            {
                date = GetPaymentDate(uri);
            }
            catch (WebException exception)
            {
                rule.LastRunErrorCode = SslCertificateExpirationDateErrorCode.Success;
                return new SendUnitTestResultRequestData()
                {
                    Result = UnitTestResult.Alarm,
                    Message = exception.Message
                };
            }

            var days = (date.Date - DateTime.Now.Date).TotalDays;
            rule.LastRunErrorCode = SslCertificateExpirationDateErrorCode.Success;
            var result = new SendUnitTestResultRequestData()
            {
                Message = string.Format(
                    "Осталось {0} дней до окончания срока действия сертификата. Срок действия {1}",
                    days,
                    date.ToString("dd.MM.yyyy"))
            };
            if (days <= rule.AlarmDaysCount)
            {
                result.Result = UnitTestResult.Alarm; 
            }
            else if (days <= rule.WarningDaysCount)
            {
                result.Result = UnitTestResult.Warning;
            }
            else
            {
                result.Result = UnitTestResult.Success;
            }
            return result;
        }
    }
}
