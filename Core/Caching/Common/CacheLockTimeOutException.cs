using System;

namespace Zidium.Core.Caching
{
    /// <summary>
    /// Исключение когда не удается получить объект блокировки в отведенное время
    /// </summary>
    public class CacheLockTimeOutException : Exception
    {
        public CacheLock Lock { get; private set; }

        public CacheLockTimeOutException(CacheLock lockObj) : base("Таймаут блокировки кэша")
        {
            if (lockObj == null)
            {
                throw new ArgumentNullException("lockObj");
            }
            Lock = lockObj;
        }
    }
}
