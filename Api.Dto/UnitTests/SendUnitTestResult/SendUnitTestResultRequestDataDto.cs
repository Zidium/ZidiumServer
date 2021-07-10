using System;
using System.Collections.Generic;

namespace Zidium.Api.Dto
{
    /// <summary>
    /// Сообщение с результатом юнит-теста
    /// </summary>
    public class SendUnitTestResultRequestDataDto
    {
        public Guid? UnitTestId { get; set; }
        
        public double? ActualIntervalSeconds { get; set; }

        public UnitTestResult? Result { get; set; }

        /// <summary>
        /// Код причины (чтобы не склеивать в одно событие разные проблемы)
        /// </summary>
        public int? ReasonCode { get; set; }

        /// <summary>
        /// Комментарий к результату юнит-теста (необязательно)
        /// </summary>
        public string Message { get; set; }

        public DateTime? NextExecutionTime { get; set; }

        public int? AttempCount { get; set; }

        public List<ExtentionPropertyDto> Properties { get; set; }
    }
}
