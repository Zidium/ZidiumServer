using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class HttpRequestUnitTestRuleDataRepository : IHttpRequestUnitTestRuleDataRepository
    {
        public HttpRequestUnitTestRuleDataRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(HttpRequestUnitTestRuleDataForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.HttpRequestUnitTestRuleDatas.Add(new DbHttpRequestUnitTestRuleData()
                {
                    Id = entity.Id,
                    RuleId = entity.RuleId,
                    Type = entity.Type,
                    Key = entity.Key,
                    Value = entity.Value
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public HttpRequestUnitTestRuleDataForRead[] GetByRuleId(Guid ruleId)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.HttpRequestUnitTestRuleDatas.AsNoTracking()
                    .Where(t => t.RuleId == ruleId)
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public void Delete(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var entity = DbGetOneById(id);
                contextWrapper.Context.HttpRequestUnitTestRuleDatas.Remove(entity);
                contextWrapper.Context.SaveChanges();
            }
        }

        private DbHttpRequestUnitTestRuleData DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.HttpRequestUnitTestRuleDatas.Find(id);
            }
        }

        private DbHttpRequestUnitTestRuleData DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Запись данных правила http-проверки {id} не найдена");

            return result;
        }

        private HttpRequestUnitTestRuleDataForRead DbToEntity(DbHttpRequestUnitTestRuleData entity)
        {
            if (entity == null)
                return null;

            return new HttpRequestUnitTestRuleDataForRead(entity.Id, entity.RuleId, entity.Type, entity.Key, entity.Value);
        }
    }
}
