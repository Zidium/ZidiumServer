using System;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class ShowSettingsTcpPortModel
    {
        public Guid UnitTestId { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool Opened { get; set; }

        public static ShowSettingsTcpPortModel Create(UnitTestForRead unitTest, IStorage storage)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }

            var rule = storage.UnitTestTcpPortRules.GetOneByUnitTestId(unitTest.Id);
            
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