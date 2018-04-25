using System;

namespace Zidium.Core.AccountsDb
{
    public interface ITokenService
    {
        Token GenerateToken(Guid accountId, Guid userId, TokenPurpose purpose, TimeSpan actualInterval);

        Token UseToken(Guid accountId, Guid token, TokenPurpose purpose);

        Token GetOneById(Guid accountId, Guid id);

        Token GetOneOrNullById(Guid accountId, Guid id);
    }
}
