using System;
using Zidium.Core.Common.Helpers;

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
            
            Data.Add("Lock object Count", lockObj.Count);
            Data.Add("Lock object Index", lockObj.Index);
            Data.Add("Locker Thread Stack", ThreadHelper.GetStackText(lockObj.Thread));
            Data.Add("Locker Thread State", lockObj.Thread.ThreadState.ToString());
        }
    }
}
