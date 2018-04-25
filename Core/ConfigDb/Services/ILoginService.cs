using System;

namespace Zidium.Core.ConfigDb
{
    public interface ILoginService
    {
        LoginInfo[] GetAllByLogin(string login);

        LoginInfo GetOneById(Guid id);

        LoginInfo GetOneOrNullById(Guid id);

        LoginInfo GetOneOrNull(Guid accountId, string login);

        LoginInfo Add(Guid id, Guid accountId, string login);

        LoginInfo UpdateLogin(Guid id, string login);

        LoginInfo UpdateLastEntryDate(Guid id, DateTime date);

        LoginInfo UpdateUserAgentTag(Guid id, Guid userAgentTag);

        void Delete(Guid id);

        string MasterPassword();
    }
}
