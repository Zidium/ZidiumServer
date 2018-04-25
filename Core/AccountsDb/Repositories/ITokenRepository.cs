using System;

namespace Zidium.Core.AccountsDb
{
    public interface ITokenRepository
    {
        Token Add(Token token);

        Token GetOneById(Guid id);

        Token GetOneOrNullById(Guid id);

        void Remove(Token token);
    }
}
