using System;
using Zidium.Storage;

namespace Zidium.Agent.AgentTasks
{
    public class DomainNamePaymentPeriodCheckResult
    {
        public DomainNamePaymentPeriodErrorCode Code { get; set; }

        public DateTime? Date { get; set; }

        public string ErrorMessage { get; set; }

        public string Html { get; set; }
    }
}
