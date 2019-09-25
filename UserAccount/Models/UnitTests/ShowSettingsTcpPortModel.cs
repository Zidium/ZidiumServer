using System;
using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class ShowSettingsTcpPortModel
    {
        public Guid UnitTestId { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool Opened { get; set; }

        public static ShowSettingsTcpPortModel Create(UnitTest unitTest)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }
            var rule = unitTest.TcpPortRule;
            if (rule == null)
            {
                throw new Exception("unittest TcpPortRule data is null");
            }
            return new ShowSettingsTcpPortModel()
            {
                UnitTestId = unitTest.Id,
                Host = rule.Host,
                Port = rule.Port,
                Opened = rule.Opened
            };
        }
    }
}