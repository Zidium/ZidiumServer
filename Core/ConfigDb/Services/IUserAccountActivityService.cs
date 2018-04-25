using System;

namespace Zidium.Core.ConfigDb
{
    public interface IUserAccountActivityService
    {
        void Add(Guid loginId, string baseUrl);
    }
}
