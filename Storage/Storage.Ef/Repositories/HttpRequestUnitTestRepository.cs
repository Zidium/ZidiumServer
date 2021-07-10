using System;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class HttpRequestUnitTestRepository : IHttpRequestUnitTestRepository
    {
        public HttpRequestUnitTestRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;


        public void Add(HttpRequestUnitTestForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.HttpRequestUnitTests.Add(new DbHttpRequestUnitTest()
                {
                    UnitTestId = entity.UnitTestId
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        // TODO Nothing to update
        public void Update(HttpRequestUnitTestForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var httpUnitTest = DbGetOneById(entity.UnitTestId);

                contextWrapper.Context.SaveChanges();
            }
        }

        public HttpRequestUnitTestForRead GetOneByUnitTestId(Guid unitTestId)
        {
            return DbToEntity(DbGetOneById(unitTestId));
        }

        public HttpRequestUnitTestForRead GetOneOrNullByUnitTestId(Guid unitTestId)
        {
            return DbToEntity(DbGetOneOrNullById(unitTestId));
        }

        private DbHttpRequestUnitTest DbGetOneOrNullById(Guid unitTestId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.HttpRequestUnitTests.Find(unitTestId);
            }
        }

        private DbHttpRequestUnitTest DbGetOneById(Guid unitTestId)
        {
            var result = DbGetOneOrNullById(unitTestId);

            if (result == null)
                throw new ObjectNotFoundException($"Http-проверка {unitTestId} не найдена");

            return result;
        }

        private HttpRequestUnitTestForRead DbToEntity(DbHttpRequestUnitTest entity)
        {
            if (entity == null)
                return null;

            return new HttpRequestUnitTestForRead(entity.UnitTestId);
        }
    }
}
