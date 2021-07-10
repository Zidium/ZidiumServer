using System;

namespace Zidium.Storage
{
    public interface IDefectRepository
    {
        void Add(DefectForAdd entity);

        void Update(DefectForUpdate entity);

        DefectForRead GetOneById(Guid id);

        DefectForRead GetOneOrNullById(Guid id);

        int GetCount();
    }
}
