using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Правило http-проверки
    /// </summary>
    public class HttpRequestUnitTestRuleForRead
    {
        public HttpRequestUnitTestRuleForRead(
            Guid id, 
            Guid httpRequestUnitTestId, 
            int sortNumber, 
            string displayName, 
            string url, 
            HttpRequestMethod method, 
            string body, 
            int? responseCode, 
            int maxResponseSize, 
            string successHtml, 
            string errorHtml, 
            int? timeoutSeconds, 
            HttpRequestErrorCode? lastRunErrorCode, 
            string lastRunErrorMessage, 
            DateTime? lastRunTime, 
            int? lastRunDurationMs, 
            bool isDeleted)
        {
            Id = id;
            HttpRequestUnitTestId = httpRequestUnitTestId;
            SortNumber = sortNumber;
            DisplayName = displayName;
            Url = url;
            Method = method;
            Body = body;
            ResponseCode = responseCode;
            MaxResponseSize = maxResponseSize;
            SuccessHtml = successHtml;
            ErrorHtml = errorHtml;
            TimeoutSeconds = timeoutSeconds;
            LastRunErrorCode = lastRunErrorCode;
            LastRunErrorMessage = lastRunErrorMessage;
            LastRunTime = lastRunTime;
            LastRunDurationMs = lastRunDurationMs;
            IsDeleted = isDeleted;
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Ссылка на настройку http-проверки
        /// </summary>
        public Guid HttpRequestUnitTestId { get; }

        /// <summary>
        /// Порядковый номер
        /// </summary>
        public int SortNumber { get; }

        /// <summary>
        /// Дружелюбное имя правила
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Урл запроса
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Метод запроса
        /// </summary>
        public HttpRequestMethod Method { get; }

        /// <summary>
        /// Содержимое Body для POST-запроса
        /// </summary>
        public string Body { get; }

        /// <summary>
        /// Код ответа, который должен быть
        /// </summary>
        public int? ResponseCode { get; }

        /// <summary>
        /// Максимальный размер ответа
        /// </summary>
        public int MaxResponseSize { get; }

        /// <summary>
        /// Фрагмент Html, который должен быть в ответе, если ошибок нет
        /// </summary>
        public string SuccessHtml { get; }

        /// <summary>
        /// Фрагмент Html который должен быть при ошибке
        /// </summary>
        public string ErrorHtml { get; }

        /// <summary>
        /// Максимальное время выполнения запроса
        /// </summary>
        public int? TimeoutSeconds { get; }

        /// <summary>
        /// Результат последнего выполнения
        /// </summary>
        public HttpRequestErrorCode? LastRunErrorCode { get; }

        /// <summary>
        /// Последняя ошибка, если была
        /// </summary>
        public string LastRunErrorMessage { get; }

        /// <summary>
        /// Время последнего запуска
        /// </summary>
        public DateTime? LastRunTime { get; }

        /// <summary>
        /// Длительность последнего запуска
        /// </summary>
        public int? LastRunDurationMs { get; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public bool IsDeleted { get; }

        public HttpRequestUnitTestRuleForUpdate GetForUpdate()
        {
            return new HttpRequestUnitTestRuleForUpdate(Id);
        }

    }
}
