using System;
using System.Collections.Generic;

namespace Zidium.Core.Limits
{
    internal class LimitDataArchiveItem
    {
        public Guid Id { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }

        /// <summary>
        /// Количество вызовов api событий за данный период
        /// </summary>
        public int EventsRequests { get; set; }

        /// <summary>
        /// Размер лога за данный период
        /// </summary>
        public Int64 LogSize { get; set; }

        /// <summary>
        /// Количество вызовов api проверок за данный период
        /// </summary>
        public int UnitTestsRequests { get; set; }

        /// <summary>
        /// Количество вызовов API метрик за данный период
        /// </summary>
        public int MetricsRequests { get; set; }

        /// <summary>
        /// Размер событий за данный период
        /// </summary>
        public Int64 EventsSize { get; set; }

        /// <summary>
        /// Размер проверок за данный период
        /// </summary>
        public Int64 UnitTestsSize { get; set; }

        /// <summary>
        /// Размер метрик за данный период
        /// </summary>
        public Int64 MetricsSize { get; set; }

        /// <summary>
        /// Затраты хранилища за данный период
        /// </summary>
        public Int64 StorageSize
        {
            get { return EventsSize + UnitTestsSize + MetricsSize + LogSize; }
        }

        /// <summary>
        /// Количество sms за данный период
        /// </summary>
        public int SmsCount { get; set; }

        /// <summary>
        /// Количество результатов по типам проверок
        /// </summary>
        public Dictionary<Guid, LimitDataArchiveUnitTestItem> UnitTestData { get; set; }

        public LimitDataArchiveUnitTestItem GetUnitTestData(Guid unitTestId)
        {
            lock (_lockObject)
            {
                LimitDataArchiveUnitTestItem result;
                UnitTestData.TryGetValue(unitTestId, out result);
                if (result == null)
                {
                    result = new LimitDataArchiveUnitTestItem();
                    UnitTestData.Add(unitTestId, result);
                }
                return result;
            }
        }

        private readonly Object _lockObject = new Object();

        public LimitDataArchiveItem()
        {
            UnitTestData = new Dictionary<Guid, LimitDataArchiveUnitTestItem>();
        }
    }
}
