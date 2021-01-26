using System;
using System.Net;
using Xunit;
using Zidium.Agent.AgentTasks;

namespace Zidium.Core.Tests.AgentTests
{
    public class SslCertificateExpirationDateCheckTests : BaseTest
    {
        [Fact]
        public void MainTest()
        {
            //var date = SslCertificateExpirationDateCheckProcessor.GetPaymentDate(new Uri("https://www.google.ru"));
            var date = SslCertificateExpirationDateCheckProcessor.GetPaymentDate(new Uri("https://www.yandex.ru"));
            date = SslCertificateExpirationDateCheckProcessor.GetPaymentDate(new Uri("https://zidium.net"));
            date = SslCertificateExpirationDateCheckProcessor.GetPaymentDate(new Uri("https://doc.alcospot.ru"));
        }

        [Fact]
        public void TooManyRedirectionsTest()
        {
            var date = SslCertificateExpirationDateCheckProcessor.GetPaymentDate(new Uri("https://owa.aps-tender.com"));
        }

        [Fact]
        public void NonExistingWebsiteTest()
        {
            Assert.Throws<WebException>(() =>
            {
                var date = SslCertificateExpirationDateCheckProcessor.GetPaymentDate(new Uri("https://ulf.aps-smart.com/"));
            });
        }
    }
}
