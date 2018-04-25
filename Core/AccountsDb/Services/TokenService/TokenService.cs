using System;
using Zidium.Core.Common;

namespace Zidium.Core.AccountsDb
{
    public class TokenService : ITokenService
    {
        protected DatabasesContext Context { get; set; }

        public TokenService(DatabasesContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public Token GenerateToken(Guid accountId, Guid userId, TokenPurpose purpose, TimeSpan actualInterval)
        {
            var accountContext = Context.GetAccountDbContext(accountId);
            var userRepository = accountContext.GetUserRepository();
            var user = userRepository.GetById(userId);

            var token = new Token()
            {
                Purpose = purpose,
                UserId = user.Id,
                SecurityStamp = user.SecurityStamp,
                EndDate = DateTime.Now.Add(actualInterval)
            };

            var userTokenRepository = accountContext.GetTokenRepository();
            token = userTokenRepository.Add(token);

            return token;
        }

        public Token UseToken(Guid accountId, Guid tokenId, TokenPurpose purpose)
        {
            var accountContext = Context.GetAccountDbContext(accountId);
            var tokenRepository = accountContext.GetTokenRepository();
            var token = tokenRepository.GetOneOrNullById(tokenId);

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

            if (token.EndDate < DateTime.Now)
            {
                throw new TokenNotValidException("Истекло время жизни токена");
            }

            // проверим пользователя
            var userRepository = accountContext.GetUserRepository();
            var user = userRepository.GetById(token.UserId);

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
            token.IsUsed = true;
            accountContext.SaveChanges();

            return token;
        }

        public Token GetOneById(Guid accountId, Guid id)
        {
            var accountContext = Context.GetAccountDbContext(accountId);
            var tokenRepository = accountContext.GetTokenRepository();
            var token = tokenRepository.GetOneById(id);
            return token;
        }

        public Token GetOneOrNullById(Guid accountId, Guid id)
        {
            var accountContext = Context.GetAccountDbContext(accountId);
            var tokenRepository = accountContext.GetTokenRepository();
            var token = tokenRepository.GetOneOrNullById(id);
            return token;
        }
    }
}
