using System;
using Zidium.Core.AccountsDb.Classes;

namespace Zidium.Core.AccountsDb
{
    internal class DefectRepository : IDefectRepository
    {
        protected AccountDbContext Context { get; set; }

        public DefectRepository(AccountDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public Defect GetOneOrNullById(Guid id)
        {
            return Context.Defects.Find(id);
        }

        public Defect GetById(Guid id)
        {
            var defect = GetOneOrNullById(id);
            if (defect == null)
            {
                throw new ObjectNotFoundException("Не удалось найти дефект " + id);
            }
            return defect;
        }
    }
}
