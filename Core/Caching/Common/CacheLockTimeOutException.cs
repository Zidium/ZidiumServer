using System;

namespace Zidium.Core.Caching
{
    /// <summary>
    /// Исключение когда не удается получить объект блокировки в отведенное время
    /// </summary>
    public class CacheLockTimeOutException : Exception
    {
        public CacheLock Lock { get; }

        public CacheLockTimeOutException(CacheLock lockObj) : base("Таймаут блокировки кэша")
        {
            if (lockObj == null)
            {
                throw new ArgumentNullException("lockObj");
            }
            Lock = lockObj;

            Data.Add("Lock object Count", lockObj.Count);
            Data.Add("Lock object Index", lockObj.Index);
            Data.Add("Locker Thread State", lockObj.Thread?.ThreadState.ToString());
            Data.Add("Object", lockObj.Response.GetType().Name);
        }
    }
}
