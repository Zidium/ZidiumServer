using System;
using System.Collections.Generic;

namespace Zidium.Storage.Ef
{
    public class DbLimitData
    {
        public DbLimitData()
        {
            UnitTestData = new HashSet<DbLimitDataForUnitTest>();
        }

        public Guid Id { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }

        public LimitDataType Type { get; set; }

        public virtual ICollection<DbLimitDataForUnitTest> UnitTestData { get; set; }

        /// <summary>
        /// Количество вызовов api событий за данный период
        /// </summary>
        public int EventsRequests { get; set; }

        /// <summary>
        /// Размер лога за данный период
        /// </summary>
        public long LogSize { get; set; }

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
        public long EventsSize { get; set; }

        /// <summary>
        /// Размер проверок за данный период
        /// </summary>
        public long UnitTestsSize { get; set; }

        /// <summary>
        /// Размер метрик за данный период
        /// </summary>
        public long MetricsSize { get; set; }

        /// <summary>
        /// Количество sms за данный период
        /// </summary>
        public int SmsCount { get; set; } 

    }
}
