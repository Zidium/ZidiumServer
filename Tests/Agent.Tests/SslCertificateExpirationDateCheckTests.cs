using System;
using System.Net;
using Xunit;
using Zidium.Agent.AgentTasks;

namespace Zidium.Agent.Tests
{
    public class SslCertificateExpirationDateCheckTests : BaseTest
    {
        [Fact]
        public void MainTest()
        {
            //var date = SslCertificateExpirationDateCheckProcessor.GetPaymentDate(new Uri("https://www.google.ru"));
            var date = SslCertificateExpirationDateCheckProcessor.GetPaymentDate(new Uri("https://yandex.ru"));
            date = SslCertificateExpirationDateCheckProcessor.GetPaymentDate(new Uri("https://zidium.net"));
        }

        [Fact]
        public void NonExistingWebsiteTest()
        {
            Assert.ThrowsAny<Exception>(() =>
            {
                var date = SslCertificateExpirationDateCheckProcessor.GetPaymentDate(new Uri("https://zidium-111111.net/"));
            });
        }
    }
}
