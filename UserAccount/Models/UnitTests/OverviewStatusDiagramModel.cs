using System;
using Zidium.Storage;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class OverviewStatusDiagramModel
    {
        public Guid UnitTestId { get; set; }

        public static OverviewStatusDiagramModel Create(UnitTestForRead unitTest)
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