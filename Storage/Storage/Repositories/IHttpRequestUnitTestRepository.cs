using System;

namespace Zidium.Storage
{
    public interface IHttpRequestUnitTestRepository
    {
        void Add(HttpRequestUnitTestForAdd entity);

        void Update(HttpRequestUnitTestForUpdate entity);

        HttpRequestUnitTestForRead GetOneByUnitTestId(Guid unitTestId);

        HttpRequestUnitTestForRead GetOneOrNullByUnitTestId(Guid unitTestId);

    }
}
