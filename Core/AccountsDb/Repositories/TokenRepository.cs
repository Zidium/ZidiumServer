using System;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    public class TokenRepository : ITokenRepository
    {
        protected AccountDbContext Context;

        public TokenRepository(AccountDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public Token Add(Token token)
        {
            if (token.Id == Guid.Empty)
            {
                token.Id = Guid.NewGuid();
                token.CreationDate = DateTime.Now;
            }
            Context.Tokens.Add(token);
            Context.SaveChanges();
            return token;
        }

        public Token GetOneById(Guid id)
        {
            var entity = GetOneOrNullById(id);

            if (entity == null)
                throw new UserFriendlyException(string.Format("Токен {0} не найден", id));

            return entity;
        }

        public void Remove(Token token)
        {
            Context.Tokens.Remove(token);
            Context.SaveChanges();
        }

        public Token GetOneOrNullById(Guid id)
        {
            return Context.Tokens.FirstOrDefault(t => t.Id == id);
        }
    }
}
