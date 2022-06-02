using System;

namespace Zidium.Storage
{
    public interface IApiKeyRepository
    {
        void Add(ApiKeyForAdd entity);

        void Update(ApiKeyForUpdate entity);

        ApiKeyForRead[] GetAll();

        ApiKeyForRead GetOneById(Guid id);

        ApiKeyForRead GetOneOrNullById(Guid id);

        void Delete(Guid id);
    }
}
