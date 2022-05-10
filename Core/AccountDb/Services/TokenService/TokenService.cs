using System;
using Zidium.Common;
using Zidium.Core.Common;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public class TokenService : ITokenService
    {
        public TokenService(IStorage storage, ITimeService timeService)
        {
            _storage = storage;
            _timeService = timeService;
        }

        private readonly IStorage _storage;
        private readonly ITimeService _timeService;

        public TokenForRead GenerateToken(Guid userId, TokenPurpose purpose, TimeSpan actualInterval)
        {
            var user = _storage.Users.GetOneById(userId);

            var token = new TokenForAdd()
            {
                Id = Ulid.NewUlid(),
                CreationDate = _timeService.Now(),
                Purpose = purpose,
                UserId = user.Id,
                SecurityStamp = user.SecurityStamp,
                EndDate = _timeService.Now().Add(actualInterval)
            };

            _storage.Tokens.Add(token);

            return _storage.Tokens.GetOneById(token.Id);
        }

        public TokenForRead UseToken(Guid tokenId, TokenPurpose purpose)
        {
            var token = _storage.Tokens.GetOneOrNullById(tokenId);

            // проверим токен
            if (token == null)
            {
                throw new TokenNotValidException("Токен не найден");
            }

            if (token.IsUsed)
            {
                throw new TokenNotValidException("Токен уже использован");
            }

            if (token.Purpose != purpose)
            {
                throw new TokenNotValidException("Неверный тип токена");
            }

            if (token.EndDate < _timeService.Now())
            {
                throw new TokenNotValidException("Истекло время жизни токена");
            }

            // проверим пользователя
            var user = _storage.Users.GetOneById(token.UserId);

            if (token.SecurityStamp != user.SecurityStamp)
            {
                // происходит когда пользователь меняет пароль
                throw new TokenNotValidException("Токен устарел");
            }

            if (user.InArchive)
            {
                throw new TokenNotValidException("Пользователь удален");
            }

            // сделаем отметку, что токен уже использован
            var tokenForUpdate = token.GetForUpdate();
            tokenForUpdate.IsUsed.Set(true);
            _storage.Tokens.Update(tokenForUpdate);

            return _storage.Tokens.GetOneById(token.Id); ;
        }

        public TokenForRead GetOneById(Guid id)
        {
            return _storage.Tokens.GetOneById(id);
        }

        public TokenForRead GetOneOrNullById(Guid id)
        {
            return _storage.Tokens.GetOneOrNullById(id);
        }
    }
}
