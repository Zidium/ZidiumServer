using System;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    public class RoleRepository:IRoleRepository
    {
        protected AccountDbContext Context { get; set; }

        public RoleRepository(AccountDbContext context)
        {
            Context = context;
        }

        public Role GetOrCreateOne(Role role)
        {
            var roleDb = Context.Roles.SingleOrDefault(x => x.Id == role.Id);
            if (roleDb == null)
            {
                return Context.Roles.Add(role);
            }
            return roleDb;
        }

        public Role GetOneById(Guid id)
        {
            return Context.Roles.Single(x => x.Id == id);
        }
    }
}
