using System;

namespace Zidium.Core.AccountsDb
{
    public interface IRoleRepository
    {
        Role GetOrCreateOne(Role role);

        Role GetOneById(Guid id);
    }
}
