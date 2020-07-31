using System;

namespace Zidium.Storage
{
    /// <summary>
    /// Правило http-проверки
    /// </summary>
    public class HttpRequestUnitTestRuleForUpdate
    {
        public HttpRequestUnitTestRuleForUpdate(Guid id)
        {
            Id = id;
            SortNumber = new ChangeTracker<int>();
            DisplayName = new ChangeTracker<string>();
            Url = new ChangeTracker<string>();
            Method = new ChangeTracker<HttpRequestMethod>();
            Body = new ChangeTracker<string>();
            ResponseCode = new ChangeTracker<int?>();
            MaxResponseSize = new ChangeTracker<int>();
            SuccessHtml = new ChangeTracker<string>();
            ErrorHtml = new ChangeTracker<string>();
            TimeoutSeconds = new ChangeTracker<int?>();
            LastRunErrorCode = new ChangeTracker<HttpRequestErrorCode?>();
            LastRunErrorMessage = new ChangeTracker<string>();
            LastRunTime = new ChangeTracker<DateTime?>();
            LastRunDurationMs = new ChangeTracker<int?>();
            IsDeleted = new ChangeTracker<bool>();
        }

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Порядковый номер
        /// </summary>
        public ChangeTracker<int> SortNumber { get; }

        /// <summary>
        /// Дружелюбное имя правила
        /// </summary>
        public ChangeTracker<string> DisplayName { get; }

        /// <summary>
        /// Урл запроса
        /// </summary>
        public ChangeTracker<string> Url { get; }

        /// <summary>
        /// Метод запроса
        /// </summary>
        public ChangeTracker<HttpRequestMethod> Method { get; }

        /// <summary>
        /// Содержимое Body для POST-запроса
        /// </summary>
        public ChangeTracker<string> Body { get; }

        /// <summary>
        /// Код ответа, который должен быть
        /// </summary>
        public ChangeTracker<int?> ResponseCode { get; }

        /// <summary>
        /// Максимальный размер ответа
        /// </summary>
        public ChangeTracker<int> MaxResponseSize { get; }

        /// <summary>
        /// Фрагмент Html, который должен быть в ответе, если ошибок нет
        /// </summary>
        public ChangeTracker<string> SuccessHtml { get; }

        /// <summary>
        /// Фрагмент Html который должен быть при ошибке
        /// </summary>
        public ChangeTracker<string> ErrorHtml { get; }

        /// <summary>
        /// Максимальное время выполнения запроса
        /// </summary>
        public ChangeTracker<int?> TimeoutSeconds { get; }

        /// <summary>
        /// Результат последнего выполнения
        /// </summary>
        public ChangeTracker<HttpRequestErrorCode?> LastRunErrorCode { get; }

        /// <summary>
        /// Последняя ошибка, если была
        /// </summary>
        public ChangeTracker<string> LastRunErrorMessage { get; }

        /// <summary>
        /// Время последнего запуска
        /// </summary>
        public ChangeTracker<DateTime?> LastRunTime { get; }

        /// <summary>
        /// Длительность последнего запуска
        /// </summary>
        public ChangeTracker<int?> LastRunDurationMs { get; }

        /// <summary>
        /// Признак удалённого
        /// </summary>
        public ChangeTracker<bool> IsDeleted { get; }

    }
}