using System;
using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public interface ITokenService
    {
        TokenForRead GenerateToken(Guid userId, TokenPurpose purpose, TimeSpan actualInterval);

        TokenForRead UseToken(Guid token, TokenPurpose purpose);

        // TODO Remove trivial method
        TokenForRead GetOneById(Guid id);

        // TODO Remove trivial method
        TokenForRead GetOneOrNullById(Guid id);
    }
}
