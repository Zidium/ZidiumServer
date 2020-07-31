using System;
using System.Linq;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class HttpRequestUnitTestRuleRepository : IHttpRequestUnitTestRuleRepository
    {
        public HttpRequestUnitTestRuleRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(HttpRequestUnitTestRuleForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.HttpRequestUnitTestRules.Add(new DbHttpRequestUnitTestRule()
                {
                    Id = entity.Id,
                    DisplayName = entity.DisplayName,
                    IsDeleted = entity.IsDeleted,
                    HttpRequestUnitTestId = entity.HttpRequestUnitTestId,
                    Body = entity.Body,
                    Url = entity.Url,
                    LastRunErrorMessage = entity.LastRunErrorMessage,
                    LastRunErrorCode = entity.LastRunErrorCode,
                    ErrorHtml = entity.ErrorHtml,
                    LastRunDurationMs = entity.LastRunDurationMs,
                    LastRunTime = entity.LastRunTime,
                    MaxResponseSize = entity.MaxResponseSize,
                    Method = entity.Method,
                    ResponseCode = entity.ResponseCode,
                    SortNumber = entity.SortNumber,
                    SuccessHtml = entity.SuccessHtml,
                    TimeoutSeconds = entity.TimeoutSeconds
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(HttpRequestUnitTestRuleForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var rule = DbGetOneById(entity.Id);

                if (entity.SortNumber.Changed())
                    rule.SortNumber = entity.SortNumber.Get();

                if (entity.DisplayName.Changed())
                    rule.DisplayName = entity.DisplayName.Get();

                if (entity.Url.Changed())
                    rule.Url = entity.Url.Get();

                if (entity.Method.Changed())
                    rule.Method = entity.Method.Get();

                if (entity.Body.Changed())
                    rule.Body = entity.Body.Get();

                if (entity.ResponseCode.Changed())
                    rule.ResponseCode = entity.ResponseCode.Get();

                if (entity.MaxResponseSize.Changed())
                    rule.MaxResponseSize = entity.MaxResponseSize.Get();

                if (entity.SuccessHtml.Changed())
                    rule.SuccessHtml = entity.SuccessHtml.Get();

                if (entity.ErrorHtml.Changed())
                    rule.ErrorHtml = entity.ErrorHtml.Get();

                if (entity.TimeoutSeconds.Changed())
                    rule.TimeoutSeconds = entity.TimeoutSeconds.Get();

                if (entity.LastRunErrorCode.Changed())
                    rule.LastRunErrorCode = entity.LastRunErrorCode.Get();

                if (entity.LastRunErrorMessage.Changed())
                    rule.LastRunErrorMessage = entity.LastRunErrorMessage.Get();

                if (entity.LastRunTime.Changed())
                    rule.LastRunTime = entity.LastRunTime.Get();

                if (entity.LastRunDurationMs.Changed())
                    rule.LastRunDurationMs = entity.LastRunDurationMs.Get();

                if (entity.IsDeleted.Changed())
                    rule.IsDeleted = entity.IsDeleted.Get();

                contextWrapper.Context.SaveChanges();
            }
        }

        public HttpRequestUnitTestRuleForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public HttpRequestUnitTestRuleForRead[] GetByUnitTestId(Guid unitTestId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.HttpRequestUnitTestRules.AsNoTracking()
                    .Where(t => t.HttpRequestUnitTestId == unitTestId)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        private DbHttpRequestUnitTestRule DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.HttpRequestUnitTestRules.Find(id);
            }
        }

        private DbHttpRequestUnitTestRule DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Правило http-проверки {id} не найдено");

            return result;
        }

        private HttpRequestUnitTestRuleForRead DbToEntity(DbHttpRequestUnitTestRule entity)
        {
            if (entity == null)
                return null;

            return new HttpRequestUnitTestRuleForRead(entity.Id, entity.HttpRequestUnitTestId, entity.SortNumber, entity.DisplayName,
                entity.Url, entity.Method, entity.Body, entity.ResponseCode, entity.MaxResponseSize, entity.SuccessHtml,
                entity.ErrorHtml, entity.TimeoutSeconds, entity.LastRunErrorCode, entity.LastRunErrorMessage,
                entity.LastRunTime, entity.LastRunDurationMs, entity.IsDeleted);
        }
    }
}
