using System;
using System.Linq;
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
                    UnitTestId = entity.UnitTestId,
                    HasBanner = entity.HasBanner,
                    LastBannerCheck = entity.LastBannerCheck
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(HttpRequestUnitTestForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var httpUnitTest = DbGetOneById(entity.UnitTestId);

                if (entity.HasBanner.Changed())
                    httpUnitTest.HasBanner = entity.HasBanner.Get();

                if (entity.LastBannerCheck.Changed())
                    httpUnitTest.LastBannerCheck = entity.LastBannerCheck.Get();

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

        public int GetHttpChecksNoBannerCount()
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.HttpRequestUnitTests
                    .Count(t => t.UnitTest.Enable &&
                                t.LastBannerCheck.HasValue &&
                                !t.HasBanner);
            }
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

            return new HttpRequestUnitTestForRead(entity.UnitTestId, entity.HasBanner, entity.LastBannerCheck);
        }
    }
}
