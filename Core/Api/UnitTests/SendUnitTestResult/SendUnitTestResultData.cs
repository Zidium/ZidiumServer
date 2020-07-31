using System;
using System.Collections.Generic;
using Zidium.Core.Common.Helpers;
using Zidium.Storage;

namespace Zidium.Core.Api
{
    /// <summary>
    /// Сообщение с результатом юнит-теста
    /// </summary>
    public class SendUnitTestResultRequestData
    {
        public Guid? UnitTestId { get; set; }

        public double? ActualIntervalSeconds { get; set; }

        public UnitTestResult? Result { get; set; }

        /// <summary>
        /// Код причины (чтобы разные причины не склеивались в одно событие)
        /// </summary>
        public int? ReasonCode { get; set; }

        /// <summary>
        /// Комментарий к результату юнит-теста (необязательно)
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Номер попытки по счёту, для неудачных попыток
        /// </summary>
        public int? AttempCount { get; set; }

        /// <summary>
        /// Время следующего выполнения проверки, если НЕ задано, то диспетчер его расчитает самостоятельно
        /// </summary>
        public DateTime? NextExecutionTime { get; set; }

        public List<ExtentionPropertyDto> Properties { get; set; }

        // TODO Move to extension type
        public long GetSize()
        {
            // Каждый результат проверки создаёт событие категории UnitTestResult
            // Поэтому считаем как для события
            long size = DataSizeHelper.DbEventRecordOverhead;
            if (Message != null)
            {
                size += Message.Length * 2;
            }
            if (Properties != null)
            {
                foreach (var property in Properties)
                {
                    if (property != null)
                    {
                        size += DataSizeHelper.DbEventParameterRecordOverhead + property.GetSize();
                    }
                }
            }
            size += DataSizeHelper.DbEventStatusRecordOverhead;
            return size;
        }
    }
}
