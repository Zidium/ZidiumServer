using System;
using Zidium.Common;

namespace Zidium.Storage.Ef
{
    internal class TokenRepository : ITokenRepository
    {
        public TokenRepository(Storage storage)
        {
            _storage = storage;
        }

        private readonly Storage _storage;

        public void Add(TokenForAdd entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                contextWrapper.Context.Tokens.Add(new DbToken()
                {
                    Id = entity.Id,
                    UserId = entity.UserId,
                    CreationDate = entity.CreationDate,
                    EndDate = entity.EndDate,
                    IsUsed = entity.IsUsed,
                    Purpose = entity.Purpose,
                    SecurityStamp = entity.SecurityStamp
                });
                contextWrapper.Context.SaveChanges();
            }
        }

        public void Update(TokenForUpdate entity)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                var token = DbGetOneById(entity.Id);

                if (entity.IsUsed.Changed())
                    token.IsUsed = entity.IsUsed.Get();

                contextWrapper.Context.SaveChanges();
            }
        }

        public TokenForRead GetOneById(Guid id)
        {
            return DbToEntity(DbGetOneById(id));
        }

        public TokenForRead GetOneOrNullById(Guid id)
        {
            return DbToEntity(DbGetOneOrNullById(id));
        }

        private DbToken DbGetOneOrNullById(Guid id)
        {
            using (var contextWrapper = _storage.GetContextWrapper())
            {
                return contextWrapper.Context.Tokens.Find(id);
            }
        }

        private DbToken DbGetOneById(Guid id)
        {
            var result = DbGetOneOrNullById(id);

            if (result == null)
                throw new ObjectNotFoundException($"Токен {id} не найден");

            return result;
        }

        private TokenForRead DbToEntity(DbToken entity)
        {
            if (entity == null)
                return null;

            return new TokenForRead(entity.Id, entity.UserId, entity.Purpose, entity.SecurityStamp, entity.CreationDate,
                entity.EndDate, entity.IsUsed);
        }
    }
}
