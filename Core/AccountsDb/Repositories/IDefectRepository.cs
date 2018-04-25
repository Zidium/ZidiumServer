using System;
using Zidium.Core.AccountsDb.Classes;

namespace Zidium.Core.AccountsDb
{
    public interface IDefectRepository
    {
        Defect GetOneOrNullById(Guid id);

        Defect GetById(Guid id);
    }
}
