using System;

namespace Zidium.Storage
{
    public class LimitDataForAdd
    {
        public Guid Id;

        public DateTime BeginDate;

        public DateTime EndDate;

        public LimitDataType Type;

        /// <summary>
        /// Количество вызовов api событий за данный период
        /// </summary>
        public int EventsRequests;

        /// <summary>
        /// Размер лога за данный период
        /// </summary>
        public long LogSize;

        /// <summary>
        /// Количество вызовов api проверок за данный период
        /// </summary>
        public int UnitTestsRequests;

        /// <summary>
        /// Количество вызовов API метрик за данный период
        /// </summary>
        public int MetricsRequests;

        /// <summary>
        /// Размер событий за данный период
        /// </summary>
        public long EventsSize;

        /// <summary>
        /// Размер проверок за данный период
        /// </summary>
        public long UnitTestsSize;

        /// <summary>
        /// Размер метрик за данный период
        /// </summary>
        public long MetricsSize;

        /// <summary>
        /// Количество sms за данный период
        /// </summary>
        public int SmsCount;
    }
}
