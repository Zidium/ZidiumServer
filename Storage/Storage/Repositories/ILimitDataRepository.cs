using System;
namespace Zidium.Storage
{
    public  interface ILimitDataRepository
    {
        void Add(LimitDataForAdd entity);

        /*
         * .QueryAll()
                    .Where(t => t.BeginDate >= date && t.Type == type)
         */
        LimitDataForRead[] Find(DateTime from, LimitDataType type);

        LimitDataForRead[] GetByType(LimitDataType type);

        LimitDataForRead GetOneOrNullByDateAndType(DateTime date, LimitDataType type);

        void RemoveOld(DateTime date, LimitDataType type);

    }
}
