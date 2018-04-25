using System;

namespace Zidium.Core.AccountsDb.Classes.UnitTests.HttpTests
{
    public class HttpRequestUnitTestRuleData
    {
        public Guid Id { get; set; }

        public Guid RuleId { get; set; }

        public virtual HttpRequestUnitTestRule Rule { get; set; }

        public HttpRequestUnitTestRuleDataType Type { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
