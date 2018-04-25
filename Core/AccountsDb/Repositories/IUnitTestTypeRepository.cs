using System;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    public interface IUnitTestTypeRepository : IAccountBasedRepository<UnitTestType>
    {
        UnitTestType GetOneOrNullByIdOrSystemName(Guid id, string systemName);

        UnitTestType GetOneOrNullBySystemName(string systemName);

        IQueryable<UnitTestType> QueryAllForGui(string search);

        IQueryable<UnitTestType> QuerySystemForGui();

        IQueryable<UnitTestType> QueryAllCustom();
    }
}
