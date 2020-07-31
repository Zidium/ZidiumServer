using System;

namespace Zidium.Storage
{
    public interface ITokenRepository
    {
        void Add(TokenForAdd entity);

        void Update(TokenForUpdate entity);

        TokenForRead GetOneById(Guid id);

        TokenForRead GetOneOrNullById(Guid id);

    }
}
