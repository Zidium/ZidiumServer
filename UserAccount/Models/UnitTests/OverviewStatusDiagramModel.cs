using System;
using Zidium.Core.AccountsDb;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class OverviewStatusDiagramModel
    {
        public Guid UnitTestId { get; set; }

        public static OverviewStatusDiagramModel Create(UnitTest unitTest)
        {
            if (unitTest == null)
            {
                throw new ArgumentNullException("unitTest");
            }
            return new OverviewStatusDiagramModel()
            {
                UnitTestId = unitTest.Id
            };
        }
    }
}