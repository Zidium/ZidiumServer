using System;

namespace Zidium.Storage
{
    public interface IHttpRequestUnitTestRuleRepository
    {
        void Add(HttpRequestUnitTestRuleForAdd entity);

        void Update(HttpRequestUnitTestRuleForUpdate entity);

        HttpRequestUnitTestRuleForRead GetOneById(Guid id);

        HttpRequestUnitTestRuleForRead[] GetByUnitTestId(Guid unitTestId);

    }
}
