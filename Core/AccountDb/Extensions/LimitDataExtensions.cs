using Zidium.Storage;

namespace Zidium.Core.AccountsDb
{
    public static class LimitDataExtensions
    {
        /// <summary>
        /// Затраты хранилища за данный период
        /// </summary>
        public static long StorageSize(this LimitDataForRead limitData)
        {
            return limitData.EventsSize + limitData.UnitTestsSize + limitData.MetricsSize + limitData.LogSize;
        }
    }
}
