using System;

namespace Zidium.Storage
{
    public interface IHttpRequestUnitTestRuleDataRepository
    {
        void Add(HttpRequestUnitTestRuleDataForAdd entity);

        HttpRequestUnitTestRuleDataForRead[] GetByRuleId(Guid ruleId);

        void Delete(Guid id);
    }
}
