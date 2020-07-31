using System;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public interface ITokenService
    {
        TokenForRead GenerateToken(Guid accountId, Guid userId, TokenPurpose purpose, TimeSpan actualInterval);

        TokenForRead UseToken(Guid accountId, Guid token, TokenPurpose purpose);

        // TODO Remove trivial method
        TokenForRead GetOneById(Guid accountId, Guid id);

        // TODO Remove trivial method
        TokenForRead GetOneOrNullById(Guid accountId, Guid id);
    }
}
