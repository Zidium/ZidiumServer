using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class ApiKeyRepository : IApiKeyRepository
    {
        public ApiKeyRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(ApiKeyForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.ApiKeys.Add(new DbApiKey()
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Value = entity.Value,
                    UpdatedAt = entity.UpdatedAt,
                    UserId = entity.UserId
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(ApiKeyForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var apiKey = DbGetOneById(entity.Id);

                if (entity.Name.Changed())
                    apiKey.Name = entity.Name.Get();

                if (entity.Value.Changed())
                    apiKey.Value = entity.Value.Get();

                if (entity.UpdatedAt.Changed())
                    apiKey.UpdatedAt = entity.UpdatedAt.Get();

                if (entity.UserId.Changed())
                    apiKey.UserId = entity.UserId.Get();

                contextWrapper.Context.SaveChanges();
            }
        }

        public ApiKeyForRead[] GetAll()
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.ApiKeys.AsNoTracking()
                    .AsEnumerable()
                    .Select(DbToEntity)
                    .ToArray();
            }
        }

        public void Delete(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var apiKey = DbGetOneById(id);
                contextWrapper.Context.ApiKeys.Remove(apiKey);
                contextWrapper.Context.SaveChanges();
            }
        }

        private DbApiKey DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.ApiKeys.Find(id);
            }
        }

        private DbApiKey DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Ключ доступа {id} не найден");

            return result;
        }

        private ApiKeyForRead DbToEntity(DbApiKey entity)
        {
            if (entity == null)
                return null;

            return new ApiKeyForRead(entity.Id, entity.Name, entity.Value, entity.UpdatedAt, entity.UserId);
        }

        public ApiKeyForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public ApiKeyForRead GetOneOrNullById(Guid id)
        {
            return DbToEntity(DbGetOneOrNullById(id));
        }
    }
}
