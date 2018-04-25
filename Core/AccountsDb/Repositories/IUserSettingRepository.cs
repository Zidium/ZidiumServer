using System;

namespace Zidium.Core.AccountsDb
{
    public interface IUserSettingRepository
    {
        string GetValue(Guid userId, string name);

        void SetValue(Guid userId, string name, string value);
    }
}
