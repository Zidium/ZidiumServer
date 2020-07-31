using System;

namespace Zidium.Core.ConfigDb
{
    public interface ILoginService
    {
        LoginInfo[] GetAllByLogin(string login);

        LoginInfo GetOneById(Guid id);

        LoginInfo GetOneOrNullById(Guid id);

        LoginInfo GetOneOrNull(Guid accountId, string login);

        void Add(Guid id, Guid accountId, string login);

        void UpdateLogin(Guid id, string login);

        void UpdateLastEntryDate(Guid id, DateTime date);

        void UpdateUserAgentTag(Guid id, Guid userAgentTag);

        void Delete(Guid id);

        string MasterPassword();
    }
}
