using System;

namespace Zidium.Core.Api.Limits.Ver2
{
    public interface ILimitService
    {
        IAccountLimitChecker GetAccountLimitChecker(Guid accountId);

        void Save();
    }
}
