using System;

namespace Zidium.Storage
{
    public interface IDefectChangeRepository
    {
        void Add(DefectChangeForAdd entity);

        DefectChangeForRead GetOneById(Guid id);

        DefectChangeForRead GetLastByDefectId(Guid defectId);

        DefectChangeForRead[] GetByDefectId(Guid defectId);

    }
}
