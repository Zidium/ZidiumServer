using System;

namespace Zidium.Storage.Ef
{
    public class DbHttpRequestUnitTestRuleData
    {
        public Guid Id { get; set; }

        public Guid RuleId { get; set; }

        public virtual DbHttpRequestUnitTestRule Rule { get; set; }

        public HttpRequestUnitTestRuleDataType Type { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

    }
}
