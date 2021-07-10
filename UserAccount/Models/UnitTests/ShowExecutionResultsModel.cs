using System;

namespace Zidium.UserAccount.Models.UnitTests
{
    public class ShowExecutionResultsModel
    {
        public Guid UnitTestId { get; set; }
        public int MaxCount { get; set; }

        public UnitTestResultEventDto[] ExecutionResults { get; set; }
    }
}