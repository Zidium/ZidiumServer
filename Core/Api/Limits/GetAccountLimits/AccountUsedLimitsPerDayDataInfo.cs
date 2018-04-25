using System;

namespace Zidium.Core.Api
{
    public class AccountUsedLimitsPerDayDataInfo
    {
        /// <summary>
        /// Количество запросов api событий, использовано
        /// </summary>
        public int EventsRequests { get; set; }

        /// <summary>
        /// Количество запросов api проверок, использовано
        /// </summary>
        public int UnitTestsRequests { get; set; }

        /// <summary>
        /// Количество вызовов api метрик, использовано
        /// </summary>
        public int MetricsRequests { get; set; }

        /// <summary>
        /// Размер событий, использовано
        /// </summary>
        public Int64 EventsSize { get; set; }

        /// <summary>
        /// Размер проверок, использовано
        /// </summary>
        public Int64 UnitTestsSize { get; set; }

        /// <summary>
        /// Размер лога, использовано
        /// </summary>
        public Int64 LogSize { get; set; }

        /// <summary>
        /// Размер метрик, использовано
        /// </summary>
        public Int64 MetricsSize { get; set; }

        /// <summary>
        /// Потраченный размер хранилища
        /// </summary>
        public Int64 StorageSize
        {
            get { return EventsSize + UnitTestsSize + MetricsSize + LogSize; }
        }

    }
}
