using System;

namespace Zidium.Storage
{
    public class LimitDataForRead
    {
        public LimitDataForRead(
            Guid id,
            DateTime beginDate,
            DateTime endDate,
            LimitDataType type,
            int eventsRequests,
            long logSize,
            int unitTestsRequests,
            int metricsRequests,
            long eventsSize,
            long unitTestsSize,
            long metricsSize,
            int smsCount)
        {
            Id = id;
            BeginDate = beginDate;
            EndDate = endDate;
            Type = type;
            EventsRequests = eventsRequests;
            LogSize = logSize;
            UnitTestsRequests = unitTestsRequests;
            MetricsRequests = metricsRequests;
            EventsSize = eventsSize;
            UnitTestsSize = unitTestsSize;
            MetricsSize = metricsSize;
            SmsCount = smsCount;
        }

        public Guid Id { get; }

        public DateTime BeginDate { get; }

        public DateTime EndDate { get; }

        public LimitDataType Type { get; }

        /// <summary>
        /// Количество вызовов api событий за данный период
        /// </summary>
        public int EventsRequests { get; }

        /// <summary>
        /// Количество вызовов api проверок за данный период
        /// </summary>
        public int UnitTestsRequests { get; }

        /// <summary>
        /// Количество вызовов api метрик за данный период
        /// </summary>
        public int MetricsRequests { get; }

        /// <summary>
        /// Размер событий за данный период
        /// </summary>
        public long EventsSize { get; }

        /// <summary>
        /// Размер проверок за данный период
        /// </summary>
        public long UnitTestsSize { get; }

        /// <summary>
        /// Размер метрик за данный период
        /// </summary>
        public long MetricsSize { get; }

        /// <summary>
        /// Размер лога за данный период
        /// </summary>
        public long LogSize { get; }

        /// <summary>
        /// Количество sms за данный период
        /// </summary>
        public int SmsCount { get; }

    }
}
