using System;

namespace Zidium.Storage
{
    public class HttpRequestUnitTestRuleDataForRead
    {
        public HttpRequestUnitTestRuleDataForRead(
            Guid id, 
            Guid ruleId, 
            HttpRequestUnitTestRuleDataType type, 
            string key, 
            string value)
        {
            Id = id;
            RuleId = ruleId;
            Type = type;
            Key = key;
            Value = value;
        }

        public Guid Id { get; }

        public Guid RuleId { get; }

        public HttpRequestUnitTestRuleDataType Type { get; }

        public string Key { get; }

        public string Value { get; }

    }
}
