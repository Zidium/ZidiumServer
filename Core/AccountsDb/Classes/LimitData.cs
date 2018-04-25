using System;
using System.Collections.Generic;
using System.Linq;

namespace Zidium.Core.AccountsDb
{
    public class LimitData
    {
        public LimitData()
        {
            UnitTestData = new HashSet<LimitDataForUnitTest>();
        }

        public Guid Id { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }

        public LimitDataType Type { get; set; }

        public virtual ICollection<LimitDataForUnitTest> UnitTestData { get; set; }

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

        public LimitDataForUnitTest GetUnitTestData(Guid unitTestId)
        {
            lock (_lockObject)
            {
                var result = UnitTestData.FirstOrDefault(t => t.UnitTestId == unitTestId);
                if (result == null)
                {
                    result = new LimitDataForUnitTest()
                    {
                        Id = Guid.NewGuid(),
                        LimitData = this,
                        UnitTestId = unitTestId
                    };
                    UnitTestData.Add(result);
                }
                return result;
            }
        }

        /// <summary>
        /// Количество sms за данный период
        /// </summary>
        public int SmsCount { get; set; } 

        private readonly object _lockObject = new object();
    }
}
